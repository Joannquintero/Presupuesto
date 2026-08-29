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
| Gráficas | **victory-native** o **react-native-gifted-charts** | Reemplaza Chart.js. Debe soportar **dona, gauge semicircular, barras verticales, barras horizontales agrupadas, barras apiladas y línea con área** (ver Módulo 2). |
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
| Bloqueada | INTEGER (bool) | default 0. Si está activa, **la categoría no se ofrece al registrar gastos de ese mes** (ver Módulo 1). No exime del recálculo de porcentajes de los ajustes de saldo |

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
- Pantalla de inicio con tarjetas de acceso a cada módulo: Ver Gastos, Nuevo Gasto, Presupuestos, Resumen, Gastos Fijos, Lista de Deseos. Cada tarjeta lleva icono, título, frase descriptiva y botón, con un color distinto por módulo.
- **Categorías** no tiene tarjeta en la Home actual, pero sí debe ser alcanzable desde la navegación (menú/tab o acceso secundario).
- Navegación inferior por tabs a los módulos principales.

### Módulo 1 — Gastos (CRUD completo)
- **Listar** gastos ordenados por fecha descendente, mostrando categoría, subcategoría, descripción, fecha y monto.
  - **Paginación de 10 registros por página**, con controles Anterior / número de página / Siguiente y el texto *"Mostrando X de Y registros"*.
  - **Fila/tarjeta de total** al final con la **suma de todos los registros filtrados** (no solo los de la página visible).
  - El badge de categoría muestra la **subcategoría si el gasto tiene una**; si no, el nombre de la categoría de presupuesto.
- **Filtrar** por: Año, Mes, SubCategoría (enum `Categoria`) y rango de fechas (fechaInicio/fechaFin).
  - Los filtros se aplican **automáticamente al cambiar cualquiera** de ellos: no hay botón "Buscar". Debe existir un botón **"Limpiar"**.
  - **Valores por defecto al entrar**: Año y Mes actuales, y Fecha Inicio y Fecha Fin en el día de hoy. "Limpiar" deja Año, Mes y Categoría en "Todos" y devuelve ambas fechas a hoy.
- **Crear / Editar** gasto con formulario validado:
  - Fecha (obligatoria, por defecto hoy). Al cambiarla se recalculan **las categorías disponibles** y **el disponible de la categoría** para el nuevo mes.
  - Categoría de presupuesto (obligatoria, selector de `CategoriaPresupuesto`).
    - **Solo se listan las categorías NO bloqueadas** en la distribución del presupuesto de ese mes. Si no hay presupuesto para el periodo, se listan todas.
    - Si la categoría seleccionada queda bloqueada tras cambiar la fecha, se limpia la selección (categoría y subcategoría).
  - SubCategoría (opcional, enum `Categoria`): **solo se muestra cuando la categoría seleccionada es la categoría de sistema**. Al cambiar a otra categoría se limpia su valor.
  - Descripción (opcional, máx. 200), con una excepción descrita abajo.
  - Monto (obligatorio, > 0, formato moneda COP), con **botones `−` y `+` que restan y suman de 1.000 en 1.000** (el `−` se deshabilita por debajo de 1.000) y máscara de miles mientras se escribe (solo dígitos).
  - **Selector de montos rápidos tipo "billete"**: botones visuales con denominaciones COP **5.000, 10.000, 20.000, 50.000, 100.000**, cada uno con color distinto por denominación y estado seleccionado; al tocarlos **asignan** el monto (no lo acumulan). Reproducir el diseño de billete (color por denominación, marca "COP", check al seleccionar). Componente reutilizable entre Crear y Editar.
  - **Descripción enlazada a gastos fijos**: cuando la categoría seleccionada es la de **"Gastos Fijos"**, el campo Descripción deja de ser texto libre y se convierte en un **desplegable con los gastos fijos activos** (`GetActivos`); al elegir uno, **se autocompleta el monto** con el del gasto fijo. Con cualquier otra categoría vuelve a ser texto libre.
  - El botón de guardar permanece **deshabilitado** mientras el monto no sea mayor a 0 o no haya categoría seleccionada, y muestra indicador de progreso al guardar.
  - En **Editar**: estado de carga inicial y mensaje *"Gasto no encontrado"* con vuelta al listado si el id no existe.
- **Eliminar** gasto con confirmación.
- **Regla de negocio al crear/editar** (`ValidarPresupuestoYDisponibilidad`):
  - Debe existir un `PresupuestoMensual` para el año/mes de la fecha del gasto; si no, lanzar error: *"No existe un presupuesto mensual configurado para {año}-{mes}. Debe crearlo antes de registrar gastos."*
  - La categoría del gasto debe tener una `DistribucionPresupuesto` en ese presupuesto; si no: *"La categoría seleccionada no tiene una distribución asignada en el presupuesto de este mes."*
  - Calcular el disponible por categoría = `distribucion.Monto - gastosActualesDeLaCategoriaEnElMes` (excluyendo el gasto en edición). **Nota:** en el código actual la validación de "excede el presupuesto" está comentada; replicar el cálculo de disponible y **dejar preparada** (pero desactivada por defecto) la validación de exceso, respetando el comportamiento actual.
- **Consulta auxiliar** `GetDisponibleCategoria(anio, mes, categoriaId, excludeGastoId?)` → devuelve el disponible para mostrar en UI (`null` si no hay presupuesto del mes o la categoría no tiene distribución).
- **Panel "Disponible en categoría"** en el formulario (crear y editar), visible en cuanto se elige categoría:
  - Muestra el disponible en formato moneda, en color normal si es positivo y **en rojo si es ≤ 0**; indicador de carga mientras se consulta.
  - Si no hay presupuesto o distribución, muestra *"Sin presupuesto asignado"* y el aviso *"Para este mes/año no existe o no tiene fondos para esta categoría."*
  - Si hay disponible, muestra además **"Valor máximo a gastar por día recomendado: $X"**, calculado como `disponible / díasRestantesDelMes`, donde `díasRestantes = díasDelMes − día de la fecha del gasto + 1` (si el resultado es ≤ 0, se usa el disponible completo). **Debe aparecer tanto en Crear como en Editar** (en la versión web solo existe en Crear: unificar).
  - Es **informativo y nunca bloquea el guardado**, en coherencia con la validación de exceso desactivada.

### Módulo 2 — Resumen Mensual

Pantalla analítica de scroll largo. Selector de **Año** (año actual y los 5 anteriores) y **Mes** en la parte superior; al cambiar cualquiera se recarga **todo** el contenido. Son **siete bloques** y todos deben migrarse:

1. **Tarjetas de indicadores** (fila superior):
   - **Total del Mes** — total gastado del periodo.
   - **Período** — nombre del mes en español.
   - **Medidor tipo gauge (semicírculo)** con el **porcentaje del presupuesto consumido** = `totalGastadoDelMes / Σ(montos de las distribuciones) × 100`. Color según semáforo: **verde ≤ 80%, ámbar > 80%, rojo > 100%**; el porcentaje se dibuja en el centro con la leyenda "del presupuesto". Si no hay presupuesto del mes, muestra *"Sin presupuesto"*.
2. **Gráfica de dona** de gastos por categoría con: `CategoriaPresupuestoId`, `CategoriaNombre`, `Total`, `Cantidad`, ordenado por total desc. El tooltip muestra importe y porcentaje.
3. **Desglose por Categoría**: lista con Categoría (badge de color), **Cantidad de gastos**, **Total** y **% sobre el total del mes**, más fila/tarjeta de totales (suma de cantidades, total del mes y 100%). En móvil, tarjetas apiladas con **barra de progreso horizontal** proporcional al porcentaje.
4. **Presupuesto vs Gasto Real**:
   - **Gráfica de barras horizontales agrupadas** con dos series por categoría: *Presupuestado* (monto de la distribución) y *Gastado* (total real del mes). El tooltip del gastado indica además el % consumido de esa categoría.
   - Debajo, **rejilla de mini tarjetas de estado por categoría**: nombre, icono de semáforo, barra de progreso (recortada al 100%) y la leyenda *"$gastado — N% de $presupuestado"*. Semáforo **verde ≤ 80%, ámbar > 80%, rojo > 100%**.
   - Si no hay presupuesto del periodo: aviso *"No hay presupuesto configurado para este período."* en lugar de la gráfica.
5. **Gastos por Día**: gráfica de **barras verticales** por día del mes (`Dia`, `Total`), ordenado por día, con etiquetas tipo "Día 1".
6. **Evolución Mensual de Gastos**: gráfica de **línea con área** del total gastado de los **últimos 6 meses** (los 5 anteriores más el seleccionado), etiquetas tipo "Ene 2026".
7. **Categorías en el Tiempo**: gráfica de **barras apiladas** de los mismos 6 meses, **una serie por categoría** con su color fijo. Se omiten las categorías sin datos en todo el rango.

Notas de implementación:
- Los bloques 6 y 7 requieren consultar el resumen de los **6 meses** (bucle sobre `GetResumenMensual`), no solo el mes seleccionado; conviene resolverlo en una única consulta agregada por rendimiento.
- El bloque 4 requiere cruzar `GetResumenMensual(anio, mes)` con las **distribuciones del presupuesto** de ese periodo.
- Nombre del mes en español (`date-fns` locale `es`).
- Los ejes abrevian importes grandes: `$1.2M`, `$250k`.
- Estado sin datos del periodo: *"No hay datos para el período seleccionado."*
- Datos vía equivalente a `GetResumenMensual(anio, mes)` → `ResumenMensualDto` (con `GastosPorCategoria` y `GastosPorDia`).

### Módulo 3 — Presupuesto Mensual (CRUD + Distribución + Saldos)
- **Listar** presupuestos ordenados por Año desc, Mes desc. Mostrar Año, Mes, Monto, Concepto y **SaldoActual** calculado = `Monto + Σ(Agregar) − Σ(Quitar)`.
  - El **Saldo Actual se colorea**: verde si es ≥ al monto del presupuesto, ámbar si es positivo pero menor, rojo si es ≤ 0.
  - Si el concepto está vacío, mostrar *"Sin concepto"* en estilo atenuado. Pie con *"Total: N registro(s)"*.
  - Tres acciones por registro: **Ajustar saldo** (Módulo 4), **Editar** y **Eliminar**.
- **Filtrar** por Año, Mes y **búsqueda por concepto** (contains).
  - Filtrado **automático al cambiar** cualquier filtro, más botón "Limpiar". Al entrar viene preseleccionado el **año actual** (mes en "Todos"). El selector de año va desde el **año siguiente** hasta 5 años atrás.
- **Crear / Editar**:
  - Año: **solo lectura**, año actual por defecto, con la nota "El año se establece automáticamente".
  - Mes: combobox (nombres en español), **mes actual por defecto** al crear.
  - Monto: formato moneda, > 0. Al cambiarlo se **recalculan en vivo los montos de todas las distribuciones**.
  - Concepto: opcional, máx. 200, con **contador de caracteres `N/200`** visible.
  - Calcular y guardar `FechaInicio` (día 1) y `FechaFin` (último día del mes) automáticamente (no visibles en UI).
  - **Validación de unicidad**: no permitir dos presupuestos para el mismo Año/Mes. Error: *"Ya existe un presupuesto para {Mes} del {Año}."*
- **Eliminar** con confirmación (borra en cascada Saldos y Distribuciones).
- **Sub-funcionalidad: Distribución del presupuesto por categorías**
  - Sección para distribuir el monto total entre categorías por **porcentaje**, presentada como lista editable de filas.
  - Campos por fila: **Categoría** (combobox), **Porcentaje** (entero 0–100, sin decimales, máx. 3 dígitos), **Monto Calculado** (solo lectura), interruptor **Bloqueada** y acción de **eliminar la fila**.
  - Botón **"Agregar Categoría"** que añade una fila con la siguiente categoría aún no utilizada, al 0%.
  - Calcular `Monto` de cada distribución = `redondear(montoTotal × porcentaje / 100, 2)`, recalculado en vivo al cambiar el porcentaje o el monto total.
  - **Fila TOTAL** al pie con el porcentaje acumulado y la suma de montos, resaltada en **verde cuando suma exactamente 100%** y en **ámbar cuando no**.
  - **Fila fija de la categoría de sistema**: se inicializa al 100% al crear y se muestra **sin combobox, sin interruptor de bloqueo y sin botón de eliminar**. Tampoco se ofrece en el combobox de las demás filas.
  - **Validaciones que bloquean el guardado** (botón deshabilitado mientras alguna falle), mostrando la lista de problemas:
    - *"La suma de los porcentajes debe ser exactamente 100% (Actual: X%)."*
    - *"No se permiten categorías duplicadas en la distribución."*
    - *"Toda distribución debe tener un porcentaje mayor a 0%."*
    - Además, el monto total debe ser mayor a 0.
  - **Bloqueada** significa que **esa categoría no se ofrece al registrar gastos de ese mes** (ver Módulo 1). No impide que los ajustes de saldo recalculen su porcentaje.
  - Al crear/editar el presupuesto, persistir/reemplazar las distribuciones asociadas.
  - En **Editar**: estado de carga inicial y mensaje *"No se encontró el presupuesto solicitado."* si el id no existe.
- **Sub-funcionalidad: Saldos (agregar/quitar)** — ver Módulo 4.

### Módulo 4 — Saldos de Presupuesto (movimientos)
- Se accede desde el listado de presupuestos y se presenta como **una sola vista/modal** con dos secciones: el **historial de movimientos** arriba y el **formulario de nuevo movimiento** abajo.
- **Listar** saldos de un presupuesto ordenados por fecha desc, mostrando categoría, monto, concepto, tipo y fecha. El tipo se distingue visualmente (Agregar en verde, Quitar en rojo) y cada fila permite eliminarse. La sección solo aparece si hay historial.
- **Crear** movimiento con: Categoría (obligatoria, se marca en error mientras no se elija), Tipo (Agregar/Quitar), Monto (> 0, con máscara de miles), Concepto (opcional, máx. 200).
- **Confirmación explícita en ambas operaciones** (no basta el diálogo de borrado genérico):
  - Al guardar: *"¿Confirmar ajuste de saldo?"* advirtiendo que *"Este cambio afectará los valores de la distribución mensual y recalculará los porcentajes automáticamente."*
  - Al eliminar un movimiento: *"¿Desea eliminar este movimiento del historial?"* advirtiendo que se revertirá el impacto en el total y se recalcularán los porcentajes.
- Tras guardar o eliminar, refrescar **el historial y el listado de presupuestos** (el Saldo Actual cambia).
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
- **Listar** todos (ordenados por fecha) y filtro **solo activos** (ordenados por concepto), este último usado por el formulario de gastos (Módulo 1).
  - **Paginación de 10 por página** con *"Mostrando X de Y registros"*.
  - **Pie con el total de los gastos fijos ACTIVOS** (los inactivos no suman).
  - Columnas: Concepto, Monto, Frecuencia (badge con color propio por frecuencia), Fecha, Categoría y Estado (**Activo / Inactivo** como badge).
- **Crear / Editar**: Concepto (máx. 200), Monto, Frecuencia (enum `FrecuenciaGasto`: Mensual/Anual/Trimestral/Semestral), Fecha (obligatoria, hoy por defecto), Activo (bool, activo por defecto).
  - **La categoría NO se pide en el formulario**: se asigna automáticamente a la categoría **"Gastos Fijos"**. Mantener este comportamiento.
- **Eliminar** con confirmación que muestre el concepto e indicador de progreso.
- Mostrar nombre de categoría y etiqueta legible de la frecuencia.

### Módulo 7 — Lista de Deseos (CRUD)
- **Listar** ordenado por **Prioridad desc**, luego por Fecha, con **paginación de 10 por página** y *"Mostrando X de Y registros"*.
  - Columnas: Concepto, Monto, Fecha Estimada (o `—` si no tiene), Categoría (badge), Prioridad y Estado (Activo / Inactivo).
  - La **prioridad se muestra como badge de color por nivel**: 9–10 rojo, 7–8 ámbar, 5–6 celeste, 3–4 gris, 1–2 gris claro.
- **Crear / Editar**: Concepto (máx. 200), Monto, Fecha (opcional), Categoría (enum `CategoriaDeseo`, 10 opciones), Prioridad (1–10, default 5), Activo (bool, activo por defecto).
  - La **prioridad se edita con un control deslizante de 1 a 10** acompañado del badge de color que refleja el valor en vivo.
- **Eliminar** con confirmación que muestre el concepto.

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
- **Colores por categoría**: NO usar un color fijo por subcategoría. La app asigna a cada **categoría de presupuesto** un color de una **paleta maestra de 12 colores**, recorriendo las categorías **ordenadas por Id**, y ese color se mantiene **estable en toda la app** (badges de listas, sectores de la dona, series de las barras apiladas). Paleta: `#4A90E2`, `#E27D60`, `#F3B562`, `#C38D9E`, `#41B3A3`, `#85CDCB`, `#E8A87C`, `#F06060`, `#5C8374`, `#9B786F`, `#7E909A`, `#A593E0`.
- **Semáforo de consumo** (gauge, mini tarjetas de estado y barras de progreso): verde ≤ 80%, ámbar > 80%, rojo > 100%.
- Listas: en móvil usar **tarjetas apiladas** (no tablas) con toda la información y acciones (editar/eliminar) accesibles, cerrando la lista con una **tarjeta de total**.
- **Paginación de 10 registros por página** en gastos, gastos fijos y lista de deseos, con el texto *"Mostrando X de Y registros"*.
- Estados de carga (spinners/skeletons), estados vacíos con icono + mensaje de ayuda, y estados de "registro no encontrado" en las pantallas de edición.
- Confirmaciones de borrado con diálogo nativo; los ajustes de saldo requieren además su propia confirmación con la advertencia del recálculo (Módulo 4).
- Formato de moneda con `Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP', maximumFractionDigits: 0 })`. **Decisión explícita**: la versión web es inconsistente (unas pantallas muestran decimales y otras no, y las máscaras de entrada usan cultura `es-CL`); en la app móvil se unifica **sin decimales** en todas las pantallas.
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
6. Las **siete visualizaciones** del Resumen: gauge de % consumido, dona por categoría, desglose con porcentajes, barras horizontales de presupuesto vs gasto real con sus tarjetas de estado, barras por día, línea de evolución de 6 meses y barras apiladas de categorías en el tiempo.
7. Componente de montos rápidos (billetes COP) y componente de paginación reutilizable.
8. README en español con instrucciones de instalación, ejecución y estructura de carpetas.
9. Al finalizar: **resumen de cambios implementados**, **plan de pasos ejecutado**, **comportamiento esperado**, **criterios de aceptación** y **duración estimada de la implementación**.

---

## 11. Criterios de aceptación

- [ ] La app funciona **100% offline** con SQLite local.
- [ ] Se pueden crear, listar, filtrar, editar y eliminar **gastos**, con validación de presupuesto/distribución existente.
- [ ] Los **filtros de gastos** se aplican automáticamente, arrancan con año/mes actual y fecha de hoy, y el listado **pagina de 10 en 10** mostrando el **total de todos los registros filtrados**.
- [ ] El formulario de gasto **oculta las categorías bloqueadas** del mes y, en la categoría "Gastos Fijos", ofrece los **gastos fijos activos** en la descripción **autocompletando el monto**.
- [ ] El formulario de gasto muestra el **disponible de la categoría** y el **valor máximo recomendado por día**, tanto al crear como al editar, **sin bloquear el guardado** cuando se excede.
- [ ] El **resumen mensual** muestra los **siete bloques**: tarjetas (total, período y **gauge de % consumido**), **dona** por categoría, **desglose** con cantidad y %, **presupuesto vs gasto real** (barras horizontales + tarjetas de estado con semáforo), **barras por día**, **línea de evolución de 6 meses** y **barras apiladas de categorías en el tiempo**.
- [ ] Se gestiona el **presupuesto mensual** (CRUD, unicidad año/mes, fechas calculadas, saldo actual con su código de color).
- [ ] La **distribución por porcentajes** calcula montos en vivo, permite agregar y quitar filas, mantiene fija la fila de la categoría de sistema y **bloquea el guardado** hasta que sume exactamente 100%, sin duplicados y sin filas en 0%.
- [ ] Una categoría marcada como **Bloqueada** desaparece del selector al registrar gastos de ese mes.
- [ ] Los **saldos** (agregar/quitar) ajustan monto total, distribución y recalculan porcentajes de forma transaccional, con reverso al eliminar, y **piden confirmación explícita** advirtiendo del recálculo.
- [ ] Las **categorías** respetan las reglas de sistema y de integridad referencial al eliminar.
- [ ] **Gastos fijos** y **lista de deseos** con CRUD, paginación, badges de estado, total de fijos activos, y prioridad editable por deslizante con color por nivel.
- [ ] Los **colores por categoría** provienen de la paleta de 12 y son estables en listas y gráficas; el **semáforo 80/100%** se aplica de forma consistente.
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
7. Implementar Resumen con sus siete bloques (empezar por dona y barras por día; luego gauge, presupuesto vs gasto real y las dos series de 6 meses).
8. Implementar Gastos Fijos y Lista de Deseos.
9. Construir navegación (tabs + stacks) y pantalla Home.
10. Pruebas manuales por módulo contra los criterios de aceptación; ajustes de UX responsive.
11. README + resumen de cambios + duración.
```
