# Prompt: Migración de "Presupuesto" (Blazor Server + .NET 8) a React Native + SQLite

> Copia este prompt completo y entrégaselo a tu agente de IA / equipo de desarrollo para migrar la aplicación web de control de gastos personales a una app móvil nativa en **React Native** con persistencia **100% local en SQLite** (sin backend ni API). La app debe replicar **todos** los módulos, reglas de negocio, validaciones y flujos de la versión web actual.

---

## 1. Rol y objetivo

Actúa como **desarrollador full stack senior especializado en React Native**. Tu tarea es **migrar íntegramente** la aplicación web **Presupuesto** (control de gastos personales, actualmente en Blazor Server + .NET 8 + EF Core + SQLite) a una **aplicación móvil React Native** que funcione **offline-first** con base de datos **SQLite local en el dispositivo**.

No debe quedar ninguna funcionalidad fuera. La app final debe cubrir el 100% de los módulos descritos en la sección 5.


## 2. Stack tecnológico objetivo

| Área | Tecnología recomendada | Notas |
| --- | --- | --- |
| Framework | **React Native** (Expo con Dev Client, o RN CLI) | Preferir **Expo** para acelerar. Si se requiere SQLite nativo puro, usar `expo-sqlite`. |
| Lenguaje | **TypeScript** (estricto) | `strict: true` en `tsconfig`. |
| Base de datos | **SQLite** local | `expo-sqlite` o `react-native-nitro-sqlite` / `op-sqlite`. |
| ORM / capa de datos | **Drizzle ORM** (recomendado) o `expo-sqlite` con capa repositorio manual | Drizzle da tipado fuerte y migraciones. |
| Navegación | **React Navigation** (Stack + Bottom Tabs) o **Expo Router** | Tabs para módulos principales, Stack para Crear/Editar. |
| Estado / datos | **TanStack Query (React Query)** + Context para preferencias | Cache y refetch de listas tras mutaciones. |
| Formularios | **React Hook Form** + **Zod** | Zod replica las validaciones de DataAnnotations. |
| UI | **React Native Paper** o **Tamagui / NativeWind** | Equivalente visual a Bootstrap 5. |
| Gráficas | **victory-native** o **react-native-gifted-charts** | Reemplaza Chart.js (dona por categoría + barras por día). |
| Iconos | **@expo/vector-icons** (Bootstrap Icons → MaterialCommunityIcons/Ionicons) | Mapear iconos equivalentes. |
| Formato moneda | `Intl.NumberFormat('es-CO')` | Moneda **COP** (peso colombiano), sin decimales en la UI. |
| Fechas | **date-fns** con locale `es` | Formateo de meses en español. |

### Reglas de arquitectura (replicar Clean Architecture)

Mantén la separación por capas equivalente a la solución .NET:

```
src/
├── domain/          → Entidades (types), enums, reglas puras. Sin dependencias.
├── application/     → DTOs, interfaces de servicios, lógica de negocio (mapeos, cálculos).
├── infrastructure/  → SQLite: conexión, esquema/migraciones, repositorios, seed data.
└── presentation/    → Screens, componentes, navegación, hooks de UI.
```

- `domain` no depende de nada.
- `application` depende solo de `domain`.
- `infrastructure` depende de `domain` y `application`.
- `presentation` depende de todas las anteriores.

---

## 3. Idioma y convenciones (obligatorio)

1. **Todo en español**: nombres de entidades, variables de dominio, textos de UI, mensajes de validación y de error.
2. **Mensajes de commit** en español siguiendo **Conventional Commits** (`feat:`, `fix:`, `refactor:`, `style:`, `docs:`, `chore:`, `test:`).
3. **Código autoexplicativo**, sin comentarios innecesarios. Usar comentarios solo para lógica compleja (equivalente a los `<summary>` de C#).
4. **No** incluir comentarios que referencien a Copilot, Gemini ni ninguna IA.
5. Nombres: entidades en singular (`Gasto`, `Categoria`), DTOs con sufijo `Dto`, servicios con interfaz e implementación.
6. Optimizar para calidad de código (SonarQube/Kiuwan): baja complejidad cognitiva, métodos cortos, sin duplicación, manejo correcto de errores y logging eficiente.
7. Toda operación de datos debe ser **asíncrona** (`async/await`) y transaccional donde la lógica lo requiera.

---

## 4. Modelo de datos (esquema SQLite)

Replica exactamente el modelo de EF Core. Crea las tablas con estas columnas, tipos, precisión y relaciones. Incluye **seed data** de categorías al inicializar la base.

### 4.1 Enums (dominio)

```ts
// Categoria (subcategoría de gasto)
enum Categoria { Alimentacion = 1, Transporte = 2, Servicios = 3, Vivienda = 4, Otros = 5 }

// CategoriaDeseo (lista de deseos)
enum CategoriaDeseo {
  Tecnologia = 1, Personales = 2, Salud = 3, Hogar = 4, Compras = 5,
  Suscripciones = 6, Transporte = 7, Alimentos = 8, Servicios = 9, Otros = 10
}

// FrecuenciaGasto (gastos fijos)
enum FrecuenciaGasto { Mensual = 1, Anual = 2, Trimestral = 3, Semestral = 4 }

// TipoMovimiento (saldos)
enum TipoMovimiento { Agregar = 1, Quitar = 2 }
```

### 4.2 Tablas

**CategoriaPresupuesto**
| Columna | Tipo | Reglas |
| --- | --- | --- |
| Id | INTEGER PK autoincrement | |
| Nombre | TEXT NOT NULL | máx. 100 |
| EsSistema | INTEGER (bool) | default 0. Si es sistema no se puede editar/eliminar |

**Gasto**
| Columna | Tipo | Reglas |
| --- | --- | --- |
| Id | INTEGER PK | |
| Fecha | TEXT/ISO NOT NULL | obligatoria |
| CategoriaPresupuestoId | INTEGER FK → CategoriaPresupuesto | obligatoria, **Restrict** (no borrar categoría con gastos) |
| SubCategoria | INTEGER NULL | enum `Categoria`, opcional |
| Descripcion | TEXT NULL | máx. 200 |
| Monto | REAL(18,2) NOT NULL | rango 0.01 – 999.999.999,99 |

**PresupuestoMensual**
| Columna | Tipo | Reglas |
| --- | --- | --- |
| Id | INTEGER PK | |
| Anio | INTEGER NOT NULL | rango 2000–2100. **Solo lectura en UI** (año actual por defecto) |
| Mes | INTEGER NOT NULL | 1–12 (combobox) |
| Monto | REAL(18,2) NOT NULL | > 0, formato moneda |
| Concepto | TEXT NULL | máx. 200 |
| FechaInicio | TEXT | calculada: primer día del mes. **No se muestra en UI** |
| FechaFin | TEXT | calculada: último día del mes. **No se muestra en UI** |

**SaldoPresupuesto** (movimientos de ajuste de saldo)
| Columna | Tipo | Reglas |
| --- | --- | --- |
| Id | INTEGER PK | |
| PresupuestoMensualId | INTEGER FK → PresupuestoMensual | **Cascade** |
| CategoriaPresupuestoId | INTEGER FK → CategoriaPresupuesto | obligatoria |
| Monto | REAL(18,2) | > 0 |
| Concepto | TEXT NULL | máx. 200 |
| Tipo | INTEGER | enum `TipoMovimiento` (Agregar/Quitar) |
| Fecha | TEXT | default ahora |

**DistribucionPresupuesto** (reparto por categoría)
| Columna | Tipo | Reglas |
| --- | --- | --- |
| Id | INTEGER PK | |
| PresupuestoMensualId | INTEGER FK → PresupuestoMensual | **Cascade** |
| CategoriaPresupuestoId | INTEGER FK → CategoriaPresupuesto | **Restrict** |
| Porcentaje | REAL(5,2) | 0–100 |
| Monto | REAL(18,2) | |
| Bloqueada | INTEGER (bool) | default 0 |

**GastoFijoRecurrente**
| Columna | Tipo | Reglas |
| --- | --- | --- |
| Id | INTEGER PK | |
| Concepto | TEXT NULL | máx. 200 |
| Monto | REAL(18,2) | |
| Frecuencia | INTEGER | enum `FrecuenciaGasto` |
| Fecha | TEXT NOT NULL | |
| CategoriaPresupuestoId | INTEGER FK → CategoriaPresupuesto | **Restrict** |
| Activo | INTEGER (bool) | default 1 |

**ListaDeseo**
| Columna | Tipo | Reglas |
| --- | --- | --- |
| Id | INTEGER PK | |
| Concepto | TEXT NULL | máx. 200 |
| Monto | REAL(18,2) | |
| Fecha | TEXT NULL | opcional |
| Categoria | INTEGER | enum `CategoriaDeseo` |
| Prioridad | INTEGER | 1–10 (default 5) |
| Activo | INTEGER (bool) | default 1 |

### 4.3 Seed data (al inicializar la BD, solo si está vacía)

```
CategoriaPresupuesto:
  1 → "Gastos Básicos" (EsSistema = true)
  2 → "Gastos Personales"
  3 → "Inversiones"
  4 → "Gastos Fijos"
  5 → "Otros"
```

Inicializar la base con el equivalente a `EnsureCreated()`: crear tablas y seed si la BD no existe. Implementar migraciones versionadas para futuras evoluciones.

---

## 5. Módulos y funcionalidades a migrar (TODOS)

### Módulo 0 — Home / Dashboard
- Pantalla de inicio con tarjetas de acceso a cada módulo: Ver Gastos, Nuevo Gasto, Presupuestos, Resumen, Gastos Fijos, Lista de Deseos.
- Navegación inferior por tabs a los módulos principales.

### Módulo 1 — Gastos (CRUD completo)
- **Listar** gastos ordenados por fecha descendente, mostrando categoría, subcategoría, descripción, fecha y monto.
- **Filtrar** por: Año, Mes, SubCategoría (enum `Categoria`) y rango de fechas (fechaInicio/fechaFin).
- **Crear / Editar** gasto con formulario validado:
  - Fecha (obligatoria).
  - Categoría de presupuesto (obligatoria, selector de `CategoriaPresupuesto`).
  - SubCategoría (opcional, enum `Categoria`).
  - Descripción (opcional, máx. 200).
  - Monto (obligatorio, > 0, formato moneda COP).
  - **Selector de montos rápidos tipo "billete"**: botones visuales con denominaciones COP **5.000, 10.000, 20.000, 50.000, 100.000**, cada uno con color distinto por denominación y estado seleccionado; al tocarlos suman/asignan el monto. Reproducir el diseño de billete (color por denominación, marca "COP", check al seleccionar).
- **Eliminar** gasto con confirmación.
- **Regla de negocio al crear/editar** (`ValidarPresupuestoYDisponibilidad`):
  - Debe existir un `PresupuestoMensual` para el año/mes de la fecha del gasto; si no, lanzar error: *"No existe un presupuesto mensual configurado para {año}-{mes}. Debe crearlo antes de registrar gastos."*
  - La categoría del gasto debe tener una `DistribucionPresupuesto` en ese presupuesto; si no: *"La categoría seleccionada no tiene una distribución asignada en el presupuesto de este mes."*
  - Calcular el disponible por categoría = `distribucion.Monto - gastosActualesDeLaCategoriaEnElMes` (excluyendo el gasto en edición). **Nota:** en el código actual la validación de "excede el presupuesto" está comentada; replicar el cálculo de disponible y **dejar preparada** (pero desactivada por defecto) la validación de exceso, respetando el comportamiento actual.
- **Consulta auxiliar** `GetDisponibleCategoria(anio, mes, categoriaId, excludeGastoId?)` → devuelve el disponible para mostrar en UI.

### Módulo 2 — Resumen Mensual
- Selector de Año (últimos 6 años) y Mes.
- Tarjetas: **Total del Mes**, **Período** (mes/año), y demás indicadores.
- **Gráfica de dona** de gastos por categoría (reemplazo de Chart.js) con: `CategoriaPresupuestoId`, `CategoriaNombre`, `Total`, `Cantidad`, ordenado por total desc.
- **Gráfica de barras** de gastos por día del mes (`Dia`, `Total`), ordenado por día.
- Nombre del mes en español (`date-fns` locale `es`).
- Datos vía equivalente a `GetResumenMensual(anio, mes)` → `ResumenMensualDto`.

### Módulo 3 — Presupuesto Mensual (CRUD + Distribución + Saldos)
- **Listar** presupuestos ordenados por Año desc, Mes desc. Mostrar Año, Mes, Monto, Concepto y **SaldoActual** calculado = `Monto + Σ(Agregar) − Σ(Quitar)`.
- **Filtrar** por Año, Mes y **búsqueda por concepto** (contains).
- **Crear / Editar**:
  - Año: **solo lectura**, año actual por defecto.
  - Mes: combobox (nombres en español).
  - Monto: formato moneda, > 0.
  - Concepto: opcional, máx. 200.
  - Calcular y guardar `FechaInicio` (día 1) y `FechaFin` (último día del mes) automáticamente (no visibles en UI).
  - **Validación de unicidad**: no permitir dos presupuestos para el mismo Año/Mes. Error: *"Ya existe un presupuesto para {Mes} del {Año}."*
- **Eliminar** con confirmación (borra en cascada Saldos y Distribuciones).
- **Sub-funcionalidad: Distribución del presupuesto por categorías**
  - Sección para distribuir el monto total entre categorías por **porcentaje**.
  - Campos: Categoría (combobox) y Porcentaje (entero 0–100, sin decimales).
  - Calcular `Monto` de cada distribución = `porcentaje% × montoTotal`.
  - Soporte de distribución **Bloqueada** (no se recalcula automáticamente).
  - Al crear/editar el presupuesto, persistir/reemplazar las distribuciones asociadas.
- **Sub-funcionalidad: Saldos (agregar/quitar)** — ver Módulo 4.

### Módulo 4 — Saldos de Presupuesto (movimientos)
- **Listar** saldos de un presupuesto ordenados por fecha desc, mostrando categoría, monto, concepto, tipo y fecha.
- **Crear** movimiento con: Monto (> 0), Concepto (opcional), Tipo (Agregar/Quitar), Categoría.
- **Lógica transaccional** al crear un saldo (usar transacción SQLite):
  1. Ajustar `PresupuestoMensual.Monto`: sumar si Agregar, restar si Quitar.
  2. Ajustar el `Monto` de la `DistribucionPresupuesto` de esa categoría (sumar/restar).
  3. **Recalcular** `Porcentaje` de **todas** las distribuciones: `round(dist.Monto / presupuesto.Monto × 100, 2)`; si `Monto ≤ 0`, porcentaje = 0.
- **Eliminar** saldo: **revertir** el efecto (operación inversa sobre Monto total y distribución) y **recalcular** porcentajes, todo transaccional.

### Módulo 5 — Categorías de Presupuesto (CRUD)
- **Listar** todas las categorías.
- **Crear**: Nombre único (validar duplicado case-insensitive → *"Ya existe una categoría con este nombre."*). `EsSistema = false`.
- **Editar**: bloquear si `EsSistema` (*"No se puede editar una categoría de sistema."*); validar nombre único.
- **Eliminar**: bloquear si:
  - `EsSistema` → *"No se puede eliminar una categoría de sistema."*
  - Tiene gastos asociados → *"No se puede eliminar la categoría porque hay gastos asociados a ella."*
  - Tiene distribuciones asociadas → *"No se puede eliminar la categoría porque está siendo utilizada en distribuciones de presupuesto."*

### Módulo 6 — Gastos Fijos Recurrentes (CRUD)
- **Listar** todos (ordenados por fecha) y filtro **solo activos** (ordenados por concepto).
- **Crear / Editar**: Concepto (máx. 200), Monto, Frecuencia (enum `FrecuenciaGasto`: Mensual/Anual/Trimestral/Semestral), Fecha (obligatoria), Categoría de presupuesto, Activo (bool).
- **Eliminar** con confirmación.
- Mostrar nombre de categoría y etiqueta legible de la frecuencia.

### Módulo 7 — Lista de Deseos (CRUD)
- **Listar** ordenado por **Prioridad desc**, luego por Fecha.
- **Crear / Editar**: Concepto (máx. 200), Monto, Fecha (opcional), Categoría (enum `CategoriaDeseo`, 10 opciones), Prioridad (1–10, default 5), Activo (bool).
- **Eliminar** con confirmación.

---

## 6. DTOs / contratos (replicar)

Crea tipos/DTOs equivalentes con mapeos manuales (sin librería de mapeo automático), tal como los `MapToDto` de la versión C#:

- `GastoDto`, `CreateUpdateGastoDto`
- `PresupuestoMensualDto` (incluye `SaldoActual` y lista de `DistribucionPresupuestoDto`), `CreateUpdatePresupuestoMensualDto`
- `SaldoPresupuestoDto`, `CreateSaldoPresupuestoDto`
- `DistribucionPresupuestoDto`
- `CategoriaPresupuestoDto` (si aplica), `CreateUpdateCategoriaPresupuestoDto`
- `GastoFijoDto`, `CreateUpdateGastoFijoDto`
- `ListaDeseoDto`, `CreateUpdateListaDeseoDto`
- `ResumenMensualDto` (con `GastoPorCategoriaDto` y `GastoPorDiaDto`)

Cada servicio de dominio (`GastoService`, `PresupuestoMensualService`, `SaldoPresupuestoService`, `CategoriaPresupuestoService`, `GastoFijoService`, `ListaDeseoService`) se implementa como una capa que recibe la conexión/repositorio SQLite e implementa su interfaz (`IGastoService`, etc.). Registrar dependencias con un contenedor ligero o Context provider.

---

## 7. Validaciones (Zod) — equivalencias

Traduce las DataAnnotations a esquemas Zod con **mensajes en español**:
- `Required` → `.min(1, "campo obligatorio")` / campo no nulo.
- `StringLength(200)` → `.max(200, "no puede exceder los 200 caracteres")`.
- `Range(0.01, ...)` para montos → `.gt(0, "El monto debe ser mayor a 0")`.
- `Range(1,12)` mes, `Range(2000,2100)` año, `Range(0,100)` porcentaje (entero), `Range(1,10)` prioridad.
- Formato de moneda en inputs de monto (máscara COP, sin decimales visibles).

---

## 8. UX / diseño responsive

- Reemplazar Bootstrap por componentes móviles nativos, manteniendo la identidad visual (colores por categoría, badges, tarjetas con sombra).
- **Colores por categoría** (mapear a la UI): Alimentación `#198754`, Transporte `#0d6efd`, Servicios `#0dcaf0`, Vivienda/Entretenimiento `#ffc107`, Otros `#6c757d`.
- Listas: en móvil usar **tarjetas apiladas** (no tablas) con toda la información y acciones (editar/eliminar) accesibles.
- Estados de carga (spinners/skeletons) y estados vacíos.
- Confirmaciones de borrado con diálogo nativo.
- Formato de moneda con `Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP', maximumFractionDigits: 0 })`.
- Selector de montos rápidos (billetes) como componente reutilizable entre Crear y Editar gasto.

---

## 9. Rutas / navegación (equivalencia)

| Web (ruta) | App (screen) |
| --- | --- |
| `/` | HomeScreen (tab) |
| `/gastos` | GastosListScreen (tab) |
| `/gastos/crear` | GastoCreateScreen |
| `/gastos/editar/{id}` | GastoEditScreen |
| `/gastos/resumen` | ResumenScreen (tab) |
| `/presupuesto-mensual` | PresupuestoListScreen (tab) |
| `/presupuesto-mensual/crear` | PresupuestoCreateScreen |
| `/presupuesto-mensual/editar/{id}` | PresupuestoEditScreen |
| `/categorias` | CategoriasScreen |
| `/gastosfijos` | GastosFijosScreen (+ crear/editar) |
| `/listadeseos` | ListaDeseosScreen (+ crear/editar) |

---

## 10. Entregables

1. Proyecto React Native + TypeScript funcional, compilable y ejecutable (`expo start` / build).
2. Base SQLite con esquema, migraciones y seed de categorías.
3. Todos los módulos de la sección 5 implementados con sus reglas de negocio y validaciones.
4. Capa de servicios/repositorios equivalente a la lógica .NET (mapeos, cálculos de saldo, recálculo de porcentajes, validaciones de presupuesto).
5. Navegación completa por tabs + stack.
6. Gráficas de dona (por categoría) y barras (por día) en el Resumen.
7. Componente de montos rápidos (billetes COP).
8. README en español con instrucciones de instalación, ejecución y estructura de carpetas.
9. Al finalizar: **resumen de cambios implementados**, **plan de pasos ejecutado**, **comportamiento esperado**, **criterios de aceptación** y **duración estimada de la implementación**.

---

## 11. Criterios de aceptación

- [ ] La app funciona **100% offline** con SQLite local.
- [ ] Se pueden crear, listar, filtrar, editar y eliminar **gastos**, con validación de presupuesto/distribución existente.
- [ ] El **resumen mensual** muestra totales, gráfica de dona por categoría y barras por día.
- [ ] Se gestiona el **presupuesto mensual** (CRUD, unicidad año/mes, fechas calculadas, saldo actual).
- [ ] La **distribución por porcentajes** calcula montos y respeta el bloqueo.
- [ ] Los **saldos** (agregar/quitar) ajustan monto total, distribución y recalculan porcentajes de forma transaccional, con reverso al eliminar.
- [ ] Las **categorías** respetan las reglas de sistema y de integridad referencial al eliminar.
- [ ] **Gastos fijos** y **lista de deseos** con CRUD, filtros y prioridades completos.
- [ ] Todos los textos, validaciones y errores están en **español**.
- [ ] Formato de moneda **COP** consistente en toda la app.
- [ ] Código limpio, tipado estricto, baja complejidad (apto SonarQube/Kiuwan).

---

## 12. Plan de pasos sugerido

1. Inicializar proyecto Expo + TypeScript, configurar linting/formato y estructura por capas.
2. Configurar SQLite (conexión, esquema, migraciones, seed de categorías).
3. Definir enums, entidades (types) y DTOs en `domain`/`application`.
4. Implementar repositorios y servicios con su lógica de negocio (empezar por Categorías y Presupuesto Mensual).
5. Implementar módulo de Gastos + validaciones de presupuesto + selector de billetes.
6. Implementar Saldos y Distribución (lógica transaccional y recálculo de porcentajes).
7. Implementar Resumen con gráficas.
8. Implementar Gastos Fijos y Lista de Deseos.
9. Construir navegación (tabs + stacks) y pantalla Home.
10. Pruebas manuales por módulo contra los criterios de aceptación; ajustes de UX responsive.
11. README + resumen de cambios + duración.
```
