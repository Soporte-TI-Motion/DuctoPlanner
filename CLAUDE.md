# Calculo ductos winUi 3 — contexto del proyecto

> Última actualización: 2026-10-21. Ver `docs/` para el detalle completo (reglas de negocio, evaluación de código, plan de migración).

## Qué es este repo

Sistema de **cotización de instalación de ductos verticales de lámina** (para edificios, no cálculo HVAC de ingeniería). Dado un edificio con N niveles y sus alturas, calcula el despiece de piezas de ducto prefabricadas, componentes/accesorios, mano de obra, flete e indirectos/viáticos, y genera una cotización con precio final.

## Los 4 proyectos del repo y su relación

| Proyecto | Qué es | Estado |
|---|---|---|
| `Calculo ductos/` | Librería .NET 8 — **motor de cálculo activo** del despiece (bin-packing de piezas por altura de piso). Entrada: `Facade.cs` → `Context/ContextDucts.cs`. | Activo, consumido por la app WinUI vía `ProjectReference`. |
| `Calculo ductos winUi 3/` | App de escritorio WinUI 3, MVVM. `StateViewModel` orquesta todo el flujo (ductos → flete → mano de obra → indirectos → guardado). | Activo, es la UI de producción. |
| `CotizadorApiVertical/` | API ASP.NET Web API clásico (.NET Framework 4.6.2), Dapper + stored procedures contra SQL Server (`Cotizador_App`). | **API activa/productiva.** Está en el `.sln` y es la que consume la app de escritorio. |
| `CotizadorVerticalApi/` | API .NET 8 con DI real, prototipo con solo 3 commits (may 2025). | **Prototipo abandonado**, no referenciado en el `.sln`, no usado por la app. Candidato a eliminar/archivar. Hay contaminación de namespaces (`namespace CotizadorVerticalApi.*` dentro de archivos de `CotizadorApiVertical/`) que confirma que el código se copió de aquí sin limpiar. |

`Ducto Planner/` es solo el empaquetado MSIX de la app de escritorio, sin lógica de negocio.

## Dato más importante para no perder: dependencia de un ERP externo (SyteLine)

El precio de los kits/materiales **no vive en `Cotizador_App`**. El stored procedure `Obtener_Catalogo_Kits` (en `BaseDeDatos/Esquema.sql`) hace un cross-database query en vivo contra `[VERS_App].[dbo].[itemprice_mst]` y `[VERS_App].[dbo].[currate_mst]` — tablas típicas del ERP **Infor SyteLine** — para obtener el precio más reciente en USD y el tipo de cambio de compra más reciente. Ningún archivo C# de este repo menciona esta dependencia. Es un requisito de infraestructura obligatorio para cualquier plan de migración (ver `docs/MIGRATION_PLAN.md`).

## Dónde está cada cosa

- **Esquema de BD real**: `BaseDeDatos/Esquema.sql` (está en **UTF-16**, usar `Get-Content -Encoding Unicode` en PowerShell o convertir antes de usar herramientas de texto plano). BD `Cotizador_App`, 30 tablas + 24 stored procedures.
- **Motor de despiece**: `Calculo ductos/Context/ContextDucts.cs` — algoritmo de apilado greedy con backtracking y reglas de "consecutividad de calibre" (`IsConsecutive`).
- **Reglas de costeo (mano de obra, indirectos, flete, margen)**: `Calculo ductos winUi 3/ViewModels/{ManPowerViewModel,IndirectsViewModel,CompleteDuctViewModel,StateViewModel}.cs`.
- **Cliente HTTP hacia la API**: `Calculo ductos winUi 3/Services/WebApi.cs`. URL base hardcodeada en `StateViewModel.cs` (IP interna).
- **Documentación generada** (reglas de negocio completas, evaluación de código priorizada, plan de migración a web con dos arquitecturas): carpeta `docs/` — `BUSINESS_RULES.md`, `CODE_REVIEW.md`, `MIGRATION_PLAN.md`.

## Hallazgos de seguridad pendientes (prioridad antes de cualquier despliegue nuevo)

- `CotizadorApiVertical/Web.config` tiene **usuario/password de SQL Server en texto plano**, en el repo desde el primer commit. Rotar la contraseña y mover a secreto fuera de control de versiones.
- La API **no tiene autenticación, autorización ni CORS** en ningún controller.
- Ver `docs/CODE_REVIEW.md` sección P0 para el detalle completo.

## Reglas de negocio clave a recordar

- El "cálculo de ductos" es despiece/logística, **no** ingeniería HVAC real (no hay CFM, velocidad de aire ni pérdida de presión).
- Alturas de pieza de ducto hardcoded: 1.20 m / 0.90 m / 0.60 m. Umbral de "doble altura": ≥4.5 m.
- Ajuste global de margen: `*0.95m` (5% de descuento, "por decisión de dirección"), en `StateViewModel` y `CompleteDuctViewModel`.
- Municipios hardcoded como "no locales": Toluca, Atlacomulco, Texcoco (`StateViewModel.IsLocalProject`).
- El motor de reglas de viáticos sí está parametrizado en BD (`Regla_Viatico_Configuracion`: Base/Multiplicador/Divisor/Redondeo), pero el significado de "Base" se resuelve por texto literal en el switch de C# (`IndirectsViewModel.ObtenerCantidad`) — frágil ante typos.
- Detalle completo de fórmulas: `docs/BUSINESS_RULES.md`.

## Plan de migración a web

Se evaluaron dos arquitecturas a petición del usuario (sin elegir una por defecto, ambas documentadas en detalle en `docs/MIGRATION_PLAN.md`):
- **A**: API .NET 8 + Dapper + Serilog, frontend Vite + Vue 3 + Tailwind. Riesgo bajo-medio, reutiliza SQL Server/SPs actuales.
- **B**: Cloudflare Workers + Pages. Riesgo alto — Workers no tiene conectividad nativa a SQL Server ni puede resolver el cross-database query al ERP SyteLine sin un gateway intermedio; requeriría decidir si se migra la BD a D1/Postgres (reescritura de SPs) o se mantiene un backend .NET como puente.

Antes de migrar con cualquiera de las dos: escribir tests de caracterización sobre `ContextDucts` (no existen tests hoy) y resolver los hallazgos de seguridad P0.
