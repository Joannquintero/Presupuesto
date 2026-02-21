
### Prompts de implementaciones

## Modulo de presupuesto

Crear plan de implementacion con los siguientes pasos:

1. Agregar un CRUD para presupuesto mensual con los siguientes campos:

- Anio (2026) - Por defecto el año actual
- Mes (Enero) - Combobox
- Monto (1000) - Decimal
- Concepto (Concepto) - Texto opcional

2. En la base de datos se debe almacenar fecha de inicio y fecha de fin del mes cuando se selecciona el mes, aun que no se muestre en la UI.

3. Acciones:
- Crear
- Editar
- Eliminar
- Listar
- Filtrar por anio y mes    
- Buscar 

--

en el formulario de presupuesto mensual, realizar los siguientes cambios:

1. El campo año debe ser solamente lectura.
2. El campo Monto debe tener un formato de moneda.
3. El campo Concepto debe tener un maximo de 200 caracteres.
4. Los mensajes de validacion deben ser en español.

--
Implementar funcionalidad para agregar o quitar saldos de presupuesto mensual. Debe tener los siguientes campos:

- Monto (1000) - Decimal
- Concepto (Concepto) - Texto opcional
- Tipo (Agregar/Quitar) - Combobox  




