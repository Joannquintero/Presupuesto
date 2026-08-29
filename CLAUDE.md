# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Proyecto

**Presupuesto** — aplicación web de control de gastos personales en **Blazor Server (.NET 8, Interactive SSR)** con **EF Core 8 + SQL Server**. UI en Bootstrap 5 + Bootstrap Icons + Chart.js. Todo el código, la UI y los mensajes están en **español**.

## Comandos

```bash
# Compilar toda la solución (archivo .slnx, requiere SDK moderno)
dotnet build Presupuesto.slnx

# Ejecutar (perfil http en :5160, https en :7051)
dotnet run --project src/Presupuesto.Web

# Migraciones EF Core: el DbContext vive en Infrastructure, la config en Web
dotnet ef migrations add <Nombre> -p src/Presupuesto.Infrastructure -s src/Presupuesto.Web
dotnet ef database update -p src/Presupuesto.Infrastructure -s src/Presupuesto.Web
```

No hay proyectos de test ni linters configurados; `dotnet build` es la única verificación automática. El SDK instalado es 10.x pero **todos los proyectos apuntan a `net8.0`** — no subir el TFM sin pedirlo.

`Program.cs` ejecuta `context.Database.Migrate()` al arrancar, así que la BD se crea/actualiza sola. La cadena de conexión está en `src/Presupuesto.Web/appsettings.json` (`ConnectionStrings:DefaultConnection`, apunta a `Server=.` / `presupuesto-01`).

## Arquitectura

Clean Architecture en `src/`, con dependencias en una sola dirección:

```
Domain          → Entities + Enums, sin dependencias
Application     → DTOs + Services (interfaz IXxxService + implementación XxxService)
Infrastructure  → PresupuestoDbContext, Migrations, DependencyInjection.AddInfrastructure()
Web             → Blazor Server: Components/Pages/<Modulo>/{Index,Create,Edit}.razor
```

### Convenciones estructurales que hay que respetar

- **Los servicios reciben `DbContext` base, no `PresupuestoDbContext`.** `DependencyInjection.cs` registra `AddScoped<DbContext>(p => p.GetRequiredService<PresupuestoDbContext>())` y cada servicio hace `context.Set<TEntity>()` en el constructor. Application no referencia Infrastructure.
- **Sin repositorios ni AutoMapper.** Los servicios consultan EF directamente y mapean con un método estático privado `MapToDto()`.
- Nunca exponer entidades a los `.razor`; siempre DTOs (`GastoDto` para leer, `CreateUpdateGastoDto` para escribir).
- Registrar cada servicio nuevo como `Scoped` en `Infrastructure/DependencyInjection.cs`.
- Las páginas Razor inyectan interfaces de servicio directamente y llevan `@rendermode InteractiveServer` (salvo `Home.razor`, que es estática).

### Modelo de dominio

El eje del sistema es `PresupuestoMensual` (único por año+mes), que se reparte en `DistribucionPresupuesto` (una fila por `CategoriaPresupuesto`, con `Porcentaje`, `Monto` calculado y flag `Bloqueada`). Reglas vigentes:

- La suma de porcentajes de las distribuciones **debe ser exactamente 100%**; la validación vive en el formulario (`PresupuestoMensual/Create.razor`, propiedad `PuedeGuardar`), no en el servicio.
- `SaldoPresupuestoService` es el único que muta montos post-creación: al agregar/quitar saldo ajusta `PresupuestoMensual.Monto`, el `Monto` de la distribución afectada y **recalcula los porcentajes de todas las distribuciones**, todo dentro de una transacción explícita. `DeleteAsync` revierte la misma operación.
- `GastoService.ValidarPresupuestoYDisponibilidad` exige que exista presupuesto del mes y una distribución para la categoría del gasto. **El tope de gasto por categoría está comentado a propósito** — solo se informa el disponible en la UI vía `GetDisponibleCategoriaAsync`.
- `CategoriaPresupuesto` con `EsSistema = true` no se puede editar ni eliminar. Las 5 categorías base ("Gastos Básicos", "Gastos Personales", "Inversiones", "Gastos Fijos", "Otros") vienen del `SeedData` del DbContext, así que cambiarlas exige una migración (ver `UpdateCategoria*` en `Infrastructure/Migrations`).
- El nombre `"Gastos Fijos"` está **acoplado por string** en `Gastos/Create.razor` y `Edit.razor` (`categoriaEsGastoFijo`): cuando el gasto es de esa categoría, la descripción pasa a ser un desplegable de `GastoFijoRecurrente` activos que autocompleta el monto.
- `Gasto.SubCategoria` (enum `Categoria`) es opcional y coexiste con `CategoriaPresupuestoId` (FK); no son lo mismo.

### Rareza heredada de SQLite

Las sumas de `decimal` se hacen como `SumAsync(g => (double)g.Monto)` y luego se castean de vuelta a `decimal` (`GastoService`, `PresupuestoMensualService`). Es un resto de la etapa SQLite; SQL Server ya no lo necesita, pero no cambiarlo de forma incidental porque altera el redondeo de los totales.

`Presupuesto.Web.csproj` todavía referencia `Microsoft.EntityFrameworkCore.Sqlite` y hay un `presupuesto.db` versionado; el proveedor real es SqlServer (`DependencyInjection.cs`).

## Estilo de código

- Identificadores, comentarios XML y `ErrorMessage` **en español**. Nada de comentarios que mencionen asistentes de IA.
- Código autoexplicativo: sin comentarios línea a línea; usar `<summary>` solo en clases/métodos no triviales.
- Todos los métodos de servicio son `async Task<T>`.
- Blazor: `EditForm` + `DataAnnotationsValidator` en formularios, `EventCallback<T>` para hijo→padre, `<style>` embebido en cada `.razor` (escapar `@` como `@@` en CSS: `@@media`, `@@keyframes`).
- Responsive: tabla `<table>` en ≥768px y tarjetas apiladas en <768px, alternando con clases `.xxx-desktop` / `.xxx-mobile` bajo `@@media`.
- Formato de moneda: `ToString("C0")`/`("C2")` para mostrar, `ToString("N0", new CultureInfo("es-CL"))` en los inputs de monto con máscara.

## Commits

Conventional Commits **en español**: `feat:`, `fix:`, `style:`, `refactor:`, `docs:`, `chore:`, `test:`.
Ej.: `feat: agregar tarjetas de montos rápidos al formulario de gastos`.

## Respuestas al usuario

`.github/copilot-instructions.md` fija el formato de trabajo esperado en este repo y aplica también aquí: responder **en español**, y para cada implementación incluir Plan Steps, comportamiento esperado, criterios de aceptación, duración estimada y un resumen de los cambios al terminar.

## Rutas

`/` · `/gastos` · `/gastos/crear` · `/gastos/editar/{id}` · `/gastos/resumen` · `/presupuesto-mensual[/crear|/editar/{id}]` · `/categorias[/crear|/editar/{id}]` · `/gastosfijos[/crear|/editar/{id}]` · `/listadeseos[/crear|/editar/{id}]`

Las funciones JS de Chart.js (`renderPieChart`, `renderHorizontalBarChart`, `renderStackedBarChart`, `renderLineChart`, `renderGaugeChart`, `renderDailyBarChart`) están definidas inline en `Components/App.razor` y se invocan por `IJSRuntime` desde `Gastos/Resumen.razor`.

## Otros archivos del repo

- `prompts.md` — historial de prompts de las implementaciones ya hechas.
- `prompt-migracion-react-native.md` — especificación completa para migrar la app a React Native + SQLite (no es código activo).
- `.github/agents/architect.agent.md` — perfil de agente arquitecto con el formato de documentos `*_ANALYSIS.md`.
