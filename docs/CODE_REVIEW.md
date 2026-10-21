# Evaluación de código y mejoras propuestas

> Generado: 2026-10-21. Basado en revisión de `Calculo ductos`, `Calculo ductos winUi 3`, `CotizadorApiVertical` y `CotizadorVerticalApi`.

## Resumen ejecutivo

El sistema funciona y tiene separación razonable de capas (UI / motor de cálculo / API / datos), pero acumula deuda técnica típica de un proyecto que creció por iteración rápida: reglas de negocio mágicas dispersas en código, manejo de errores que oculta fallos, un proyecto duplicado sin limpiar, y un hallazgo de seguridad real (credenciales en texto plano). Antes de migrar a web conviene resolver los puntos **P0** de esta lista, ya que de lo contrario se arrastrarían a la nueva arquitectura.

## P0 — Seguridad (resolver antes de cualquier despliegue nuevo)

1. **Credenciales SQL en texto plano en el repo**: `CotizadorApiVertical/Web.config` tiene usuario/password de SQL Server commiteados desde el primer commit. **Acción**: rotar la contraseña inmediatamente y mover el connection string a un secreto (Azure Key Vault, variables de entorno, `dotnet user-secrets` en dev).
2. **API sin autenticación/autorización ni CORS**: ningún controller tiene `[Authorize]`; cualquier cliente que alcance la URL puede leer y escribir cotizaciones. **Acción**: añadir API key o JWT antes de exponer la API fuera de la red interna, obligatorio si se migra a la nube.
3. **Logging de datos sensibles**: `QuoterService` loguea el connection string completo y el payload entero de la cotización en nivel Debug. **Acción**: sanitizar logs, nunca loguear connection strings.

## P0.5 — Dependencia oculta de un ERP externo (hallazgo del esquema de BD)

21. **Acoplamiento fuerte, no documentado en el código C#, a un ERP externo (SyteLine)**: el SP `Obtener_Catalogo_Kits` hace cross-database query en vivo contra `[VERS_App].[dbo].[itemprice_mst]` y `[VERS_App].[dbo].[currate_mst]` para obtener precio y tipo de cambio de cada kit. Ningún archivo C# de este repo menciona esta dependencia — un desarrollador que solo lea `CompleteDuctViewModel`/`Mapper.cs` asumiría que el precio viene de `Kit_Catalogo`. **Acción**: documentar esta dependencia explícitamente (ya hecho en `BUSINESS_RULES.md`) y tratarla como un requisito de infraestructura de primera clase en cualquier migración — no es opcional, es cómo se calcula el costo real de cada cotización.
22. **Procedimiento duplicado por error de tipeo**: `Obtener_Viaticos_Coizacion` (sin la "t") es una copia exacta de `Obtener_Viaticos_Cotizacion`, código muerto en la BD que debería eliminarse.
23. **SQL comentado dentro de un SP activo** (`Obtener_Flete`): la versión anterior de la consulta quedó comentada dentro del cuerpo del procedimiento en producción — mismo patrón de "código muerto sin limpiar" visto en la capa C#.
24. **`ORDER BY 8 DESC`** en `Obtener_Flete`: ordena por posición ordinal de columna en vez de por nombre — fragiliza el SP ante cualquier cambio en el `SELECT`.
25. **Referencia a tabla posiblemente huérfana**: `Obtener_Catalogo_Ventilacion` consulta `TipoVentilacion_Catalogo`, tabla que no aparece en el script de esquema exportado — verificar si existe en la BD real o si el SP está roto/sin uso.
26. **Posible duplicación de catálogo de rentabilidad**: existen dos tablas casi idénticas, `Rentabilidad_Catalogo` (decimal(6,2)) y `Rentabilidad_MO` (decimal(6,4)), con la misma PK `RentabilidadMOId` — sugiere una migración de catálogo a mitad de camino; confirmar cuál es la fuente de verdad antes de migrar.

## P1 — Correctness / robustez

4. **Manejo de errores que oculta fallos silenciosamente**: múltiples `catch (Exception) { }` vacíos o que devuelven objetos parcialmente construidos sin loguear, en `ContextDucts.CalculateDuctsByFloor`, `GetFloors`/`GetDuct`, `Extensions.SumDuctPieces`. Esto hace que errores de cálculo se traduzcan silenciosamente en cotizaciones vacías o incorrectas en vez de fallar visiblemente.
5. **Transacción inconsistente en `QuoterService.InsertQuote`**: la inserción de cotización usa una transacción SQL explícita, pero algunos repos (mano de obra, indirectos) atrapan sus propias excepciones y devuelven un resultado "fallido" sin abortar el flujo — el commit puede ocurrir aunque una parte haya fallado silenciosamente. Se rompe la garantía de atomicidad que la transacción pretende dar.
6. **Status codes HTTP engañosos**: todos los controllers devuelven `Ok(response)` (HTTP 200) incluso cuando `response.StatusCode = 500` internamente. Cualquier cliente HTTP estándar (o un futuro frontend Vue) interpretará esto como éxito. **Acción**: usar `IHttpActionResult`/`IActionResult` reales (`BadRequest`, `NotFound`, `StatusCode(500)`).
7. **Null-reference sin verificar**: `QuoterService.GetQuoteById` usa `FirstOrDefault()` repetidamente sin comprobar null antes de acceder a propiedades.
8. **División entera no validada**: `ObtenerCantidadBase` hace `(basedias * indirect.Multiplier) / indirect.Divider` sin validar `Divider != 0` ni decidir explícitamente si debe truncar o redondear — riesgo de truncar cantidades de viáticos o lanzar `DivideByZeroException`.
9. **Riesgo de bucle/backtracking frágil** en `CalculateCommonLevel`/`CalculateDoubleLevel`/`CalculateDischargeLevel`: la condición de salida depende de reglas heurísticas (`IsConsecutive`) más que de una cota explícita; aunque hay un límite de 3 reintentos, la lógica es difícil de razonar y no tiene pruebas automatizadas que la respalden.
10. **Falta de pruebas unitarias**: no se encontró carpeta de tests en ninguno de los 4 proyectos. El motor de despiece (`ContextDucts`) tiene lógica combinatoria compleja (pila + backtracking + reglas de consecutividad) — es exactamente el tipo de código que más se beneficia de tests de regresión antes de tocarlo en una migración.

## P2 — Mantenibilidad / deuda técnica

11. **Reglas de negocio "mágicas" hardcodeadas sin constantes nombradas**: alturas de ducto (1.20/0.90/0.60 m), holguras de descarga (2.3/2.42/1.5 m), compensación de chimenea (0.9 m), umbral de doble altura (4.5/5.0 m), ajuste global de margen (0.95), municipios "no locales" (Toluca/Atlacomulco/Texcoco por texto). Esto dificulta auditar o cambiar reglas de negocio sin leer el código fuente completo.
12. **Switch por texto literal en español para indirectos** (`ObtenerCantidad`, ~40 casos con strings como `"Hoteli"`, `"Alimentos c/comprobantei"`): extremadamente frágil ante un typo en el catálogo de BD o un cambio de nombre — fallaría silenciosamente (caso no encontrado = cantidad no calculada) en vez de con un error claro.
13. **Proyecto duplicado sin limpiar**: `CotizadorVerticalApi` es un prototipo abandonado que convive con `CotizadorApiVertical` en el repo, generando confusión sobre cuál mantener. Además hay contaminación cruzada de namespaces (`namespace CotizadorVerticalApi.*` en archivos de `CotizadorApiVertical/`) que confirma copy-paste sin refactorizar.
14. **Código muerto/comentado** en `Facade.cs`, `Mapper.cs` (bloque de ~40 líneas comentado), `ManPowerViewModel`/`IndirectsViewModel` (herramientas comentadas). Debe eliminarse, no dejarse "por si acaso" (git conserva el historial).
15. **Duplicación de lógica**: `InitDuctsCounter()` existe idéntico en `ContextDucts.cs` y `Dictionaries.cs`; las tres variantes de `CalculateXLevel` (common/double/discharge) replican casi la misma lógica con pequeñas variaciones — candidatas a refactor a un único método parametrizado.
16. **Acoplamiento fuerte / falta de DI**: en la API, cada Service instancia sus repos con `new XRepository()` en el constructor en vez de inyección de dependencias (contrasta con `CotizadorVerticalApi`, que sí usaba DI) — dificulta testear y reemplazar implementaciones. En la app, `StateViewModel` conoce y orquesta directamente el estado interno de todos los sub-ViewModels ("God ViewModel").
17. **Inconsistencia de patrón MVVM**: `CompleteDuctViewModel` implementa `INotifyPropertyChanged` manualmente mientras el resto usa `CommunityToolkit.Mvvm.ObservableObject`.
18. **Nombres con errores tipográficos que ya son "contrato"**: `heightAvaible`, `NeedChimmey`, literales de indirectos con errores de tipeo — riesgoso de corregir sin romper comparaciones de texto, pero debe documentarse o normalizarse con cuidado.
19. **Sin paginación** en `GetLastQuotes`/catálogos — no es un problema hoy, pero escalará mal si la tabla de cotizaciones crece.
20. **Sin validación de entrada en la API**: ningún controller valida `ModelState` ni rangos de los parámetros antes de pasarlos a los stored procedures.

## Lo que sí está bien hecho

- Acceso a datos 100% vía Dapper + stored procedures parametrizados — cero riesgo de SQL injection en la capa de datos.
- Separación clara entre motor de cálculo (librería reutilizable) y UI.
- Uso de transacciones explícitas para la operación crítica (`InsertQuote`), aunque con el bug de atomicidad descrito en el punto 5.
- Logging con log4net ya integrado (aunque con fugas de datos sensibles a corregir).

## Priorización sugerida antes de migrar

1. Rotar credenciales expuestas (P0.1) — inmediato, independiente de la migración.
2. Documentar/congelar las reglas de negocio mágicas como constantes nombradas y, si es posible, extraerlas a configuración/BD (P2.11) — esto es trabajo que vale la pena hacer *durante* la migración, ya que de todos modos se reescribirá la capa de negocio.
3. Escribir tests de caracterización sobre `ContextDucts` **antes** de tocarlo, para no perder reglas de negocio al migrar (apoya P1.10).
4. Decidir el destino de `CotizadorVerticalApi` (eliminar o archivar) para no arrastrar confusión a la migración.
