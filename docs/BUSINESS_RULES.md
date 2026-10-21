# Documentación funcional — Cotizador de Ductos Verticales

> Generado: 2026-10-21. Cubre los 4 proyectos del repo: `Calculo ductos` (motor de cálculo), `Calculo ductos winUi 3` (app de escritorio), `CotizadorApiVertical` (API activa) y `CotizadorVerticalApi` (prototipo abandonado).
>
> **Nota importante**: las fórmulas de precio/margen reales viven en *stored procedures* de SQL Server que no están en este repositorio (caja negra). Esta documentación cubre todo lo que es inferible del código fuente C#. Pendiente: incorporar el esquema/SPs de BD cuando se compartan.

## 1. Naturaleza del "cálculo de ductos"

El sistema **no realiza cálculo de ingeniería HVAC** (no hay fórmulas de caudal de aire, velocidad, pérdida de presión ni normativa SMACNA). Es un problema de:

1. **Despiece**: dado un edificio con N niveles de altura variable, determinar cuántas piezas prefabricadas de ducto (de alturas fijas: 1.20 m, 0.90 m, 0.60 m) y qué accesorios (soportes, puertas, aspersores, chimenea, etc.) se necesitan para construir la columna vertical de ducto.
2. **Costeo comercial**: traducir ese despiece + mano de obra + viáticos + flete en una cotización con precios, aplicando un margen de rentabilidad y un ajuste de dirección.

## 2. Motor de cálculo (`Calculo ductos/`)

Punto de entrada: `Facade.CalculateDuctsByFloor(string paramsJson)` → `Context/ContextDucts.cs`.

### 2.1 Catálogo de piezas y "peso" (calibre relativo)

`Params/DuctPiece.cs`:

| Tipo | Altura (m) | Peso |
|---|---|---|
| A2, B2, B2F | 0.60 | 2 |
| B3, B3F | 0.90 | 3 |
| B4, B4F, C4, S4 | 1.20 | 4 |

Las alturas son hardcodeadas en `Config/Ducts.cs` (catálogos `GetConfigCommonLevel`, `GetConfigLastLevel`, `GetConfigDoubleLevel`, `GetConfigDischargeLevel`, `GetAllDucts`) — no son parametrizables desde BD/UI.

### 2.2 Algoritmo de apilado (bin-packing greedy con backtracking)

`ContextDucts.CalculateDucts` (apilado base):
```csharp
while (availableHeight >= duct.Height)
{
    stack.Push(duct);
    availableHeight -= duct.Height;
}
```

Selección de layout según altura disponible del piso (`PositionDucts`):
```csharp
if (heightAvaible >= 4.5m)
    CalculateDoubleLevel(...)   // piso de doble altura
else
    CalculateCommonLevel(...)   // piso normal
```

**Regla de consecutividad de calibre** (`IsConsecutive`): dos piezas vecinas en la pila no pueden diferir en más de 1 de "peso", y el último nivel no puede terminar en peso 4. Si el apilado inicial no cumple esto, el algoritmo hace **backtracking** (máximo 3 iteraciones): saca la pieza más grande repetida y reintenta con la siguiente pieza disponible.

**Holguras de descarga** (constantes hardcodeadas):
```csharp
heightAvaible -= ((typeDischarge != guilloutine) ? 2.3m
                  : (needAntiImpact ? 2.42m : 1.5m));
```
- 2.3 m: descarga tipo "discharge"
- 1.5 m: guillotina normal
- 2.42 m: guillotina + anti-impacto

**Compensación por chimenea en último piso**: `heightAvaible += 0.9m` antes de recalcular.

### 2.3 Cálculo de componentes/accesorios

`ContextDucts.CalculateComponents`:
```csharp
XN (soporte normal):  Count = duct.floors.Count - 1
XNF (soporte final):  Count = 1
Sprinkler:             Count = NeedSprinkler ? (gates / 2) + 1 : 0
Gate:                  Count = gates   // gates = suma de piezas tipo C4
Guillotine:            1 si el primer piso tiene descarga tipo guillotina, si no 0
Container:             Count = 2  (fijo)
```
Los soportes normales/finales se ajustan después recorriendo los pisos según si el piso es "doble normal" (altura 4.5–5.0 m) o "doble excedido" (≥5.0 m), con constantes mágicas (1/2/3) sin documentar.

### 2.4 Regla de pieza extra de seguridad

`Utils/Extensions.cs` — comentario explícito en el código: *"Se agrega regla de tener un B2 extra para cualquier eventualidad"*. Siempre se incrementa en 1 el conteo final de piezas B2, sin importar el cálculo.

## 3. Costeo y mano de obra (`Calculo ductos winUi 3/`)

### 3.1 Costo de materiales

`CompleteDuctViewModel.CalculateSubTotalPrice`:
```csharp
SubTotalPrice += (kit.Price * kit.ExchangeRate * duct.Count);
```
Cada tipo de pieza/componente se asocia por nombre a un "Kit" del catálogo (precio y tipo de cambio vienen de la API). No hay un factor de desperdicio/merma explícito en el código — si existe, está implícito en el precio del kit que entrega el backend.

### 3.2 Mano de obra / jornadas efectivas

`ManPowerViewModel.CalculateWorkDays`:
```csharp
WorkDaysBase         = Ceiling(floors.Count / 2.5)
WorkDaysDobleFloors  = floors.Count(altura >= 4.5m) * 0.5
WorkDaysExtraFloors  = floors.Count > 10 ? 1 : 0
WorkDayForeign       = IsLocalProject() ? 0 : 1
NoWorkDays           = WorkDaysBase / 7
```
`EfectiveWorkDayModel.TotalWorkDays = Ceiling(WorkDaysBase + WorkDayForeign + WorkDaysExtraFloors + WorkDaysDobleFloors + WorkDayDueDate)`.

Costo de mano de obra por recurso = `PrecioTotal * rentability.Rentability`.

**Regla de proyecto local vs foráneo** (`StateViewModel.IsLocalProject`): los municipios **Toluca, Atlacomulco y Texcoco** (Estado de México) se consideran explícitamente "no locales" aunque pertenezcan al estado "México" — hardcoded por nombre de texto.

### 3.3 Indirectos / viáticos

`IndirectsViewModel.CalculateIndirects` recorre cada recurso humano y cada indirecto obligatorio de la zona, calculando cantidades vía un switch por **texto literal del concepto en español** (`ObtenerCantidad`), por ejemplo:
```csharp
case "Hoteli": case "Hidratacióni":
    cantidad = (TotalEfectiveDaysInstaller + TotalNoWorkDaysInstaller) * TotalResourceInstaller;
case "Alimentos c/comprobantei":
    cantidad = (TotalForeingResourceInstaller * 3) * (...);
```
y para indirectos genéricos (`ObtenerCantidadBase`):
```csharp
cantidadTotal = (basedias * indirect.Multiplier) / indirect.Divider;  // división entera
```
`CalculateTools()` agrega automáticamente "2% de MO" como herramienta menor.

**Regla de eventos de viaje** (`GetTotalEvents`):
```csharp
floorsCount <= 20 ? 1 : 1 + Ceiling((floorsCount - 20) / 10.0)
```
Es decir: 1 evento de viaje cada 10 pisos adicionales por arriba de 20.

### 3.4 Ajuste global de margen

En `StateViewModel.TotalPrice` y `CompleteDuctViewModel`:
```csharp
total *= 0.95m   // 5% de descuento, comentario: "por decisión de dirección"
```

## 4. API (`CotizadorApiVertical/`) — reglas visibles en C#

Toda la capa de datos usa Dapper + stored procedures (sin SQL concatenado). Las fórmulas de precio reales están en los SPs (fuera del repo). Reglas de negocio visibles en C#:

- `QuoteParam` trae defaults de negocio: `NecesitaIzaje = true`, `ZonaId = 1`, `Diametro = 24"`, `NombreEjecutivo = "NO ASIGNADO"`, `SiteRef = "VERS"`.
- `QuoterService.InsertQuote` inserta flete / mano de obra / viáticos **solo si el usuario los definió** (ifs condicionales), todo dentro de una única transacción SQL.
- Cada cotización se versiona (`NumeroVersion` devuelto por el SP `Insertar_Cotizacion`) — una misma PT puede tener múltiples versiones.

### Endpoints expuestos

| Método | Ruta | Función |
|---|---|---|
| GET | `api/quoter` | Últimas cotizaciones |
| GET | `api/quoter/{id}` | Detalle de cotización |
| POST | `api/quoter` | Crear/versionar cotización completa |
| GET | `api/catalog` | Catálogos combinados (puertas, niveles, propósitos, láminas, camiones, ubicaciones, recursos, rentabilidades, indirectos, zonas, kits, herramientas) |
| GET | `api/Freight?idLocalidad=&idTipoCamion=` | Costo de flete |
| POST | `api/Log` | Bitácora remota de errores del cliente |

### Esquema real confirmado (`BaseDeDatos/Esquema.sql`, BD `Cotizador_App`)

30 tablas confirmadas. Las más relevantes para el negocio:

- **`Cotizacion`** — cabecera: `PT` (proyecto), `NombreEjecutivo`, `Diametro`, `PropositoId`, `TipoLaminaId`, `NecesitaAspersor`, `NecesitaSistemaDD`, `RentabilidadMOId` (default 1), `NecesitaIzaje` (default 1), `ZonaId` (default 1), `TipoContenedorId`.
- **`VersionCotizacion`** — versionado real: `CotizacionId`, `NumeroVersion`, `SiteRef`. Confirmado por el SP `Insertar_Cotizacion` (ver abajo).
- **`Nivel`** — un registro por nivel/piso: `Altura`, `Cantidad`, `NecesitaPuerta`, `TipoNivelId`, `NecesitaChimenea`, `TipoPuertaId`, `TipoDescargaId`, `NecesitaAntiImpacto`, `TipoGuillotinaId`.
- **`Mano_De_Obra`** — `CotizacionId`, `RecursoId`, `TipoRecursoId` (sin cantidades/costos propios; el costo sale de `Recurso_Catalogo.SalarioPorJornada`).
- **`Viatico_Cotizacion`** — `Concepto`, `Cantidad`, `PrecioUnitario`, `PoliticaViaticosId`, `TipoRecursoId`.
- **`Politica_Viaticos_Catalogo`** — catálogo de conceptos de viático por zona (`ZonaId`, `UnidadMedidaId`, `Monto`, `EsObligatorio`).
- **`Regla_Viatico_Configuracion`** — **motor de reglas real** detrás del switch de C# `ObtenerCantidad`/`ObtenerCantidadBase`: por cada `PoliticaViaticosId`+`UbicacionId` define `Base` (nvarchar, nombre de la variable base: días efectivos, recursos foráneos, etc.), `Multiplicador`, `Divisor` y `RedondeoId`. Es decir, la fórmula `(basedias * Multiplicador) / Divisor` sí está parametrizada en BD, pero el *significado* de `Base` (a qué variable de negocio corresponde el string) sigue resuelto por texto en el C# — no hay un enum/id compartido.
- **`Flete`** — `TipoCamionId` + `LocalidadId` → `Precio`, único por combinación (`UQ_Flete_TipoCamion_Localidad`). Existe `Flete_Backup` (tabla de respaldo manual, sin FKs, indicio de operación manual de datos).
- **`Kit_Catalogo`** — `ItemIdSyteLine`, `CategoriaId`, `PropositoId`, `TipoLaminaId`. **No almacena precio propio.**
- **`Herramienta_Catalogo`**, **`Recurso_Catalogo`** (con `SalarioPorJornada`), **`Rentabilidad_Catalogo`**/`Rentabilidad_MO` (parecen duplicadas, ver Code Review), catálogos geográficos (`Entidad_Catalogo`→`Municipio_Catalogo`→`Localidad_Catalogo`).
- **`CargaCRM`** — tabla de integración/sincronización hacia un CRM externo, ligada a `VersionCotizacionId`, con `Estado`, `MensajeResultado`, `Enviado` — confirma que hay un proceso (externo a este repo) que empuja cotizaciones aprobadas a un CRM.

### Hallazgo clave: el precio de los kits viene de un ERP externo (SyteLine) en tiempo real

El SP `Obtener_Catalogo_Kits` **no usa un precio almacenado en `Cotizador_App`**. Hace un cross-database query contra `[VERS_App].[dbo].[itemprice_mst]` y `[VERS_App].[dbo].[currate_mst]` (nombres de tabla típicos de Infor SyteLine ERP):

```sql
WITH LastCurrency AS (
  SELECT from_curr_code, buy_rate, sell_rate,
         ROW_NUMBER() OVER (PARTITION BY from_curr_code ORDER BY eff_date DESC) AS rn
  FROM [VERS_App].[dbo].[currate_mst]
  WHERE from_curr_code = 'USD'
),
Ultimos AS (
  SELECT item, unit_price2 AS precio, effect_date AS fecha, curr_code,
         ROW_NUMBER() OVER (PARTITION BY item ORDER BY effect_date DESC) AS rn
  FROM [VERS_App].[dbo].[itemprice_mst]
  WHERE item IN (SELECT ItemIdSyteLine FROM Kit_Catalogo) AND curr_code = 'USD'
)
SELECT k.KitId, ..., u.precio [Precio], l.buy_rate [TipoCambio], ...
FROM Ultimos u JOIN Kit_Catalogo k ON k.ItemIdSyteLine = u.item ...
WHERE u.rn = 1
```

Esto es **crítico para la migración**: el precio de cada kit (`kit.Price`) y el tipo de cambio (`kit.ExchangeRate`) usados en `CompleteDuctViewModel.CalculateSubTotalPrice` (ver sección 3.1) se leen en vivo del ERP SyteLine en cada cotización, tomando el precio más reciente en USD y la tasa de compra más reciente. Cualquier arquitectura nueva necesita conectividad a esa base de datos del ERP (o a una réplica/API que la exponga) — no es un dato local del cotizador.

### Otros hallazgos del esquema

- `Insertar_Cotizacion` **auto-versiona por `PT`**: busca la última `CotizacionId` con el mismo `PT`, lee su `NumeroVersion` y lo incrementa — confirma que "guardar cotización" siempre crea una fila nueva (nunca actualiza), y el versionado es por número de proyecto.
- Procedimiento **duplicado por typo**: `Obtener_Viaticos_Coizacion` (falta la "t") coexiste con `Obtener_Viaticos_Cotizacion` (correcto) — ambos con el mismo cuerpo, uno es código muerto.
- `Obtener_Flete` tiene un bloque entero de la consulta anterior comentado dentro del propio SP (antes de la versión activa que usa `TOP 1 ... ORDER BY 8 DESC`, es decir, ordena por posición de columna, no por nombre).
- `Obtener_Catalogo_Ventilacion` referencia una tabla `TipoVentilacion_Catalogo` que **no existe en este script de esquema** — o el `CREATE TABLE` no se incluyó en el export, o es una tabla huérfana/un SP roto.

## 5. Relación entre proyectos

- `CotizadorVerticalApi` (.NET 8, prototipo, 3 commits) fue abandonado; `CotizadorApiVertical` (.NET Framework 4.6.2) es la API activa y la única referenciada en la solución y consumida por la app de escritorio. Hay evidencia de que código se copió del prototipo al proyecto legacy sin limpiar namespaces (`namespace CotizadorVerticalApi.*` dentro de archivos de `CotizadorApiVertical/`).
- `Calculo ductos` (librería) es el motor de cálculo activo, consumido por `Calculo ductos winUi 3` vía `ProjectReference` — no es un vestigio.
