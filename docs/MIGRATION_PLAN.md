# Plan de migración a web

> Generado: 2026-10-21. Objetivo: transformar el cotizador de ductos (hoy app de escritorio WinUI 3 + API ASP.NET Framework) en una aplicación web. Se detallan **dos arquitecturas candidatas** a petición explícita: (A) API .NET + Dapper + Serilog con frontend Vite+Vue3+Tailwind, y (B) Cloudflare Workers + Pages. No se recomienda una sobre otra; se comparan al final.
>
> Prerrequisito compartido por ambas rutas: resolver los hallazgos P0 de `CODE_REVIEW.md` (credenciales expuestas, falta de auth) antes o durante la migración, no después.

## Línea base a preservar

Independientemente de la arquitectura elegida, la migración debe preservar:
- El motor de despiece de `Calculo ductos/Context/ContextDucts.cs` (algoritmo de apilado + reglas de consecutividad) — es el activo de negocio más valioso y más riesgoso de reescribir sin tests.
- Las reglas de costeo (mano de obra, indirectos, flete, margen 0.95) documentadas en `BUSINESS_RULES.md`.
- Los endpoints/contratos consumidos hoy por la app de escritorio, si esta debe coexistir temporalmente con la web.
- Las 56 stored procedures de SQL Server (ya revisadas en `BaseDeDatos/Esquema.sql`) — en su mayoría son `SELECT *` simples de catálogo, no requieren reescritura, solo un nuevo cliente de acceso a datos.

### Hallazgo que condiciona ambas arquitecturas: dependencia de un ERP externo (SyteLine)

El esquema de BD revela que **el precio de los kits/materiales no vive en `Cotizador_App`**: el SP `Obtener_Catalogo_Kits` hace un cross-database query en vivo contra `[VERS_App].[dbo].[itemprice_mst]` y `[VERS_App].[dbo].[currate_mst]` (tablas típicas de Infor SyteLine) para traer precio en USD y tipo de cambio más recientes. Esto es un requisito de infraestructura obligatorio para cualquier arquitectura nueva, y es el punto que más diferencia a las dos rutas:
- **Arquitectura A (.NET)**: trivial de preservar — un `SqlConnection`/Dapper puede seguir haciendo el mismo `JOIN` entre bases si ambas están en el mismo servidor SQL Server (o vía linked server), o se puede exponer un segundo repositorio apuntando a `VERS_App`.
- **Arquitectura B (Cloudflare Workers)**: **no es trivial**. Workers no puede hacer cross-database query nativo de SQL Server ni mantener un `JOIN` entre dos bases on-prem. Requeriría: (a) un servicio intermedio (API .NET o Azure Function) que siga resolviendo este query contra SQL Server y lo exponga por HTTP al Worker, o (b) una sincronización periódica (ETL) de `itemprice_mst`/`currate_mst` hacia D1/Postgres, lo que introduce latencia de datos (el precio dejaría de ser "tiempo real" salvo que se sincronice con alta frecuencia). Esto refuerza que la Arquitectura B requiere mantener parte del backend .NET como gateway hacia el ERP, incluso si el resto se mueve a Workers.

## Arquitectura A — API .NET + Dapper + Serilog / Frontend Vite + Vue 3 + Tailwind

### A.1 Backend

1. **Nuevo proyecto** `CotizadorApi` en .NET 8 (ASP.NET Core minimal API o controllers), reemplazando `CotizadorApiVertical` (.NET Framework) y eliminando/archivando `CotizadorVerticalApi` (prototipo abandonado, pero puede servir de punto de partida ya que usa DI y .NET 8).
2. **Capas**:
   - `CotizadorApi.Api` — controllers/endpoints (minimal API o MVC controllers), validación de entrada (FluentValidation o DataAnnotations), middleware de manejo global de excepciones (reemplaza los try/catch repetidos por capa).
   - `CotizadorApi.Application` — equivalente a los actuales `Facades`/`Services`, orquestación de casos de uso (InsertQuote, GetQuoteById, etc.), inyectado vía DI (reemplaza los `new XRepository()`).
   - `CotizadorApi.Data` — repositorios con Dapper (se conserva el patrón actual, que ya es correcto), llamando a los mismos stored procedures si se mantiene la BD, o a SQL inline parametrizado si se decide prescindir de SPs.
   - `CotizadorApi.Domain` — port del motor de cálculo (`Calculo ductos`), con **tests de caracterización** escritos antes de tocar la lógica (ver P1.10 en CODE_REVIEW).
3. **Logging**: reemplazar log4net por **Serilog**, con sinks a archivo/consola en dev y a un agregador (Seq, Application Insights, o Elastic) en producción. Enriquecer con `CorrelationId` por request para trazar el flujo completo de una cotización.
4. **Seguridad**: autenticación JWT (si hay usuarios/ejecutivos que inician sesión) o API key simple si es uso interno; HTTPS obligatorio; mover connection string a `appsettings.{Environment}.json` + variables de entorno / Key Vault.
5. **Configuración de negocio**: extraer las constantes mágicas (alturas de ducto, holguras, margen 0.95, municipios "no locales") a una sección de configuración (`appsettings.json` o tabla de BD), no hardcoded.
6. **Base de datos**: mantener SQL Server + Dapper (ya validado, sin riesgo de SQL injection); evaluar migrations con un enfoque simple (scripts versionados con algo como DbUp, ya que no hay EF) dado que el acceso es con SPs.

### A.2 Frontend

1. **Stack**: Vite + Vue 3 (Composition API + `<script setup>`) + TypeScript opcional (recomendado aunque pediste JS) + Tailwind CSS + Pinia para estado global (equivalente al `StateViewModel` actual).
2. **Estructura de wizard**: replicar el flujo actual (Niveles → Ductos → Flete → Mano de obra → Indirectos → Resumen/Concepto final) como pasos de un wizard con Vue Router o un componente de steps, con el estado centralizado en un store Pinia (`useQuoteStore`) que reemplaza a `StateViewModel`.
3. **Cliente HTTP**: `fetch`/`axios` con interceptor de errores (reemplaza `Services/WebApi.cs`), manejo de loading states (reemplaza `BusyService.cs`).
4. **Exportación a Excel**: evaluar librería JS (`exceljs` o `sheetjs`) para reemplazar `ExcelFile.cs`.
5. **Deploy**: build estático servido desde IIS/Nginx o un storage con CDN (Azure Static Web Apps, S3+CloudFront, etc.), separado del backend.

### A.3 Fases sugeridas

| Fase | Entregable |
|---|---|
| 1 | Nueva API .NET 8 con mismos endpoints/contratos que la actual, consumiendo la misma BD (SPs sin cambios). Tests de caracterización del motor de cálculo. Resolver P0 de seguridad. |
| 2 | Frontend Vue 3 que replica el wizard actual contra la nueva API, en paralelo a la app de escritorio (ambas consumiendo el mismo backend). |
| 3 | Extraer reglas de negocio hardcoded a configuración/BD; reemplazar el switch de indirectos por texto con un catálogo tipado (id en vez de string). |
| 4 | Retirar `CotizadorApiVertical`, `CotizadorVerticalApi` y, si aplica, la app de escritorio. |

### A.4 Esfuerzo/riesgo

- **Riesgo bajo-medio**: reutiliza SQL Server, SPs y el patrón Dapper existente; el equipo ya conoce C#/.NET.
- **Esfuerzo principal**: reescribir el motor de cálculo con tests, y reconstruir toda la UI en Vue (wizard + tablas + exportación Excel).

## Arquitectura B — Cloudflare Workers + Pages

### B.1 Backend (Workers)

1. **Runtime**: Cloudflare Workers (JS/TypeScript), sin Dapper ni SQL Server directo — Workers no tiene conectividad nativa a SQL Server on-prem.
2. **Bloqueante adicional confirmado**: ni siquiera migrando `Cotizador_App` a D1/Postgres se resuelve el problema completo, porque el precio de los kits depende de un **tercer sistema (ERP SyteLine, BD `VERS_App`)** vía cross-database query (ver hallazgo arriba). Habría que decidir también cómo sincronizar o exponer esos datos del ERP — es un sistema adicional fuera del alcance de este repo.
3. **Base de datos — decisión crítica**: dado que el stack actual es SQL Server con stored procedures, hay dos caminos:
   - (a) **Mantener SQL Server** y acceder vía un proxy HTTP (Cloudflare Hyperdrive solo soporta Postgres/MySQL, **no SQL Server** a la fecha de este documento) — requeriría una capa intermedia (una API .NET mínima o Azure SQL con un gateway) que Workers consuma por HTTP, lo cual anula buena parte del beneficio de "todo serverless".
   - (b) **Migrar la BD** a **Cloudflare D1** (SQLite) o Postgres compatible con Hyperdrive — implica reescribir todas las stored procedures a SQL estándar o a lógica en el propio Worker, lo cual es un esfuerzo de migración de datos y lógica no trivial (los SPs no están en este repo; hay que extraerlos primero).
3. **Lógica de negocio**: portar el motor de cálculo de C# a TypeScript (reescritura completa de `ContextDucts`, no un port directo — distinto lenguaje, distinto paradigma de colecciones).
4. **Logging**: Cloudflare Workers Logs / Logpush hacia un destino externo (no hay Serilog en este ecosistema); para logging estructurado usar `console.log` con JSON + Logpush, o un servicio tipo Axiom/Better Stack.
5. **Autenticación**: Cloudflare Access o JWT propio verificado en el Worker.

### B.2 Frontend (Pages)

1. Mismo stack de UI (Vue 3 + Tailwind) pero desplegado como sitio estático en Cloudflare Pages, consumiendo los Workers como API.
2. Integración nativa Pages+Workers simplifica CI/CD (un solo repo, un solo `wrangler.toml`/`wrangler deploy`).

### B.3 Fases sugeridas

| Fase | Entregable |
|---|---|
| 1 | Extraer y documentar todas las stored procedures actuales (requiere acceso a BD) para poder decidir si migran a D1/Postgres o si se mantiene SQL Server detrás de un gateway. |
| 2 | Reescribir el motor de cálculo en TypeScript con tests (paralelo a la versión C#, comparando resultados para validar paridad). |
| 3 | Construir Workers con los mismos endpoints, inicialmente contra SQL Server vía gateway (ruta más rápida) o ya contra D1/Postgres si se decidió migrar datos. |
| 4 | Frontend en Pages, wizard igual al de Arquitectura A. |
| 5 | Si se migró a D1/Postgres: plan de migración de datos histórico (cotizaciones existentes) con validación de integridad. |

### B.4 Esfuerzo/riesgo

- **Riesgo alto**: implica cambiar de lenguaje (C#→TS) para toda la lógica de negocio, y potencialmente cambiar de motor de BD (SQL Server→D1/Postgres), lo cual depende de stored procedures que aún no se han revisado.
- **Beneficio**: infraestructura más barata y con mejor edge-latency/escalado automático; sin servidores que mantener.
- **Bloqueante actual**: no se puede comprometer un plan preciso de BD en esta arquitectura hasta tener el DDL + contenido de las stored procedures, porque determina si la migración de datos es trivial (SQL Server se queda, solo se agrega gateway) o mayor (reescribir SPs a otro motor).

## Comparación

| Criterio | A: .NET + Dapper + Vue3 | B: Cloudflare Workers + Pages |
|---|---|---|
| Reutiliza SQL Server y SPs actuales sin cambios | Sí | Solo con gateway intermedio (parcialmente serverless) |
| Reutiliza conocimiento del equipo (C#) | Sí, en backend | Solo en frontend (Vue); backend en TS |
| Reescritura del motor de cálculo | Recomendable (con tests) pero mismo lenguaje | Obligatoria y en otro lenguaje — mayor riesgo de divergencia de reglas |
| Costo de infraestructura | Requiere servidor/hosting .NET (IIS, Azure App Service) | Más barato en runtime, pero puede requerir mantener SQL Server igual si no se migra la BD |
| Tiempo estimado para MVP equivalente | Medio | Alto (depende de decisión de BD) |
| Riesgo de perder reglas de negocio en la migración | Medio (mismo lenguaje, menor fricción) | Alto (cambio de lenguaje y posible cambio de motor de BD) |

## Siguiente paso recomendado (independiente de la arquitectura)

1. Compartir el DDL/esquema de BD y, si es posible, el contenido de las stored procedures — ambas arquitecturas lo necesitan para precisar costo y riesgo, y la Arquitectura B no puede planearse con precisión sin esto.
2. Escribir tests de caracterización sobre `ContextDucts` usando casos reales de cotizaciones existentes, como red de seguridad para cualquiera de las dos rutas.
3. Resolver los hallazgos P0 de seguridad (`CODE_REVIEW.md`) antes de exponer cualquier nueva API a internet.
