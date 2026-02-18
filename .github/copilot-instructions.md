# Copilot Instructions — Presupuesto

## Descripción del Proyecto

**Presupuesto** es una aplicación web de control de gastos personales construida con **Blazor Server** y **.NET 8**. Permite registrar, consultar, filtrar, editar y eliminar gastos, así como visualizar un resumen mensual con gráfica de dona por categoría.

---

## Stack Tecnológico

| Tecnología            | Versión         | Uso                        |
| --------------------- | --------------- | -------------------------- |
| .NET                  | 8.0             | Framework principal        |
| Blazor Server         | Interactive SSR | UI con componentes Razor   |
| Entity Framework Core | 8.0             | ORM / acceso a datos       |
| SQLite                | —               | Base de datos local        |
| Bootstrap             | 5.x             | Framework CSS              |
| Bootstrap Icons       | 1.11.3          | Iconografía                |
| Chart.js              | 4.4.1           | Gráfica de dona en resumen |

---

## Arquitectura (Clean Architecture)

```
Presupuesto.slnx
└── src/
    ├── Presupuesto.Domain          → Entidades y Enums (sin dependencias externas)
    ├── Presupuesto.Application     → DTOs, Interfaces y Servicios de negocio
    ├── Presupuesto.Infrastructure  → DbContext, Inyección de dependencias, EF Core
    └── Presupuesto.Web             → Blazor Server (Pages, Components, Layout)
```

### Reglas de dependencia

- `Domain` no depende de ningún otro proyecto.
- `Application` depende solo de `Domain`.
- `Infrastructure` depende de `Domain` y `Application`.
- `Web` depende de todos los anteriores.

---

## Estructura de Carpetas Clave

```
src/
├── Presupuesto.Domain/
│   ├── Entities/Gasto.cs            # Entidad principal
│   └── Enums/Categoria.cs           # Enum de categorías de gasto
│
├── Presupuesto.Application/
│   ├── DTOs/
│   │   ├── CreateUpdateGastoDto.cs  # DTO para crear/actualizar
│   │   ├── GastoDto.cs              # DTO de lectura
│   │   └── ResumenMensualDto.cs     # DTO de resumen + GastoPorCategoriaDto
│   └── Services/
│       ├── IGastoService.cs         # Interfaz del servicio
│       └── GastoService.cs          # Implementación
│
├── Presupuesto.Infrastructure/
│   ├── Data/PresupuestoDbContext.cs  # DbContext con seed data
│   └── DependencyInjection.cs       # Extensión AddInfrastructure()
│
└── Presupuesto.Web/
    ├── Program.cs                   # Entry point
    ├── Components/
    │   ├── App.razor                # Root HTML + Chart.js script
    │   ├── Routes.razor
    │   ├── _Imports.razor
    │   ├── Layout/
    │   │   ├── MainLayout.razor
    │   │   └── NavMenu.razor
    │   ├── Pages/
    │   │   ├── Home.razor           # Página de inicio
    │   │   └── Gastos/
    │   │       ├── Index.razor      # Lista con filtros + CRUD
    │   │       ├── Create.razor     # Formulario de creación + billetes
    │   │       ├── Edit.razor       # Formulario de edición + billetes
    │   │       └── Resumen.razor    # Resumen mensual + gráfica
    │   └── Shared/
    │       └── GastosTable.razor    # Tabla compartida de gastos
    └── wwwroot/                     # Archivos estáticos
```

---

## Convenciones de Código

### Idioma

- El código fuente (variables, métodos, clases) está en **español**.
- Los comentarios XML (`<summary>`) están en **español**.
- Los mensajes de validación (`ErrorMessage`) están en **español**.
- Los **mensajes de commit** siempre deben estar en **español** y seguir el formato **Conventional Commits**:
  - `feat: <descripción>` — Nueva funcionalidad
  - `fix: <descripción>` — Corrección de errores
  - `style: <descripción>` — Cambios de estilos o formato (sin cambiar lógica)
  - `refactor: <descripción>` — Refactorización de código
  - `docs: <descripción>` — Cambios en documentación
  - `chore: <descripción>` — Tareas de mantenimiento
  - `test: <descripción>` — Agregar o modificar pruebas
  - Ejemplos:
    - `feat: agregar tarjetas de montos rápidos al formulario de gastos`
    - `fix: corregir filtro de categorías en la lista de gastos`
    - `style: mejorar diseño responsive de la tabla de desglose`

### Naming

- **Entidades**: sustantivos en singular (`Gasto`, `Categoria`).
- **DTOs**: sufijo `Dto` (`GastoDto`, `CreateUpdateGastoDto`).
- **Servicios**: prefijo `I` para interfaz (`IGastoService`) e implementación sin prefijo (`GastoService`).
- **Páginas Blazor**: PascalCase (`Create.razor`, `Resumen.razor`).

### Patrones

- **DTO Pattern**: nunca exponer entidades directamente a la UI. Usar DTOs.
- **Inyección de dependencias**: registrar en `DependencyInjection.cs` como Scoped.
- **Async/Await**: todos los métodos de servicio son `async Task<T>`.
- **Mapeo manual**: usar métodos estáticos `MapToDto()` en el servicio (sin AutoMapper).

### Blazor

- Usar `@rendermode InteractiveServer` en páginas interactivas.
- Usar `EditForm` + `DataAnnotationsValidator` para formularios.
- Usar `EventCallback<T>` para comunicación hijo → padre.
- Estilos con `<style>` scoped dentro de cada componente `.razor`.
- En CSS dentro de Razor, escapar `@` con `@@` (ej: `@@keyframes`, `@@media`).

### Diseño Responsive

- **Desktop** (≥768px): tablas clásicas con `<table>`.
- **Móvil** (<768px): tarjetas apiladas (`div` cards) con la misma información.
- Usar clases CSS de visibilidad (`.xxx-desktop` / `.xxx-mobile`) con `display: none/block` controlado por `@media`.
- Los componentes de montos rápidos usan diseño de **billetes** con colores por denominación.

---

## Categorías de Gasto

```csharp
public enum Categoria
{
    Alimentacion = 1,
    Transporte = 2,
    Servicios = 3,
    Entretenimiento = 4,
    Otros = 5
}
```

### Colores asociados

| Categoría       | Badge CSS              | Color Hex |
| --------------- | ---------------------- | --------- |
| Alimentacion    | `bg-success`           | `#198754` |
| Transporte      | `bg-primary`           | `#0d6efd` |
| Servicios       | `bg-info`              | `#0dcaf0` |
| Entretenimiento | `bg-warning text-dark` | `#ffc107` |
| Otros           | `bg-secondary`         | `#6c757d` |

---

## Cómo Ejecutar

```bash
cd src/Presupuesto.Web
dotnet run --urls "http://localhost:5100"
```

La base de datos SQLite (`presupuesto.db`) se crea automáticamente con `EnsureCreated()` y datos semilla.

---

## Rutas de la Aplicación

| Ruta                  | Página  | Descripción                 |
| --------------------- | ------- | --------------------------- |
| `/`                   | Home    | Dashboard de inicio         |
| `/gastos`             | Index   | Lista de gastos con filtros |
| `/gastos/crear`       | Create  | Formulario de nuevo gasto   |
| `/gastos/editar/{id}` | Edit    | Formulario de edición       |
| `/gastos/resumen`     | Resumen | Resumen mensual con gráfica |

---

## Guías para Nuevas Funcionalidades

1. **Nueva entidad**: crear en `Domain/Entities/`, agregar `DbSet` en `PresupuestoDbContext`, crear DTOs en `Application/DTOs/`.
2. **Nuevo servicio**: crear interfaz en `Application/Services/`, implementación que reciba `DbContext`, registrar en `DependencyInjection.cs`.
3. **Nueva página**: crear `.razor` en `Web/Components/Pages/`, agregar ruta en `NavMenu.razor`.
4. **Nueva categoría**: agregar valor al enum `Categoria`, actualizar todos los `switch` expressions (badge class, color, bar class).
