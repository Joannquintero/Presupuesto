
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

-- 
Implementar la funcionalidad para distribuir el presupuesto mensual entre diferentes categorias. Tener en cuenta lo siguiente:

1. Se debe mostrar una seccion para distribuir el presupuesto mensual entre diferentes categorias.

2. La distribucion se debe hacer con base en porcentajes sobre el monto total del presupuesto mensual.

4. Los campos deben ser:
- Categoria (Combobox)
- Porcentaje (Decimal) - Debe ser entre 0 y 100, sin decimales.
- Monto (Decimal) - Solo lectura y calculado automaticamente

5. El porcentaje debe ser entre 0 y 100.

6. El monto debe ser el porcentaje sobre el monto total del presupuesto mensual.

7. El total de los porcentajes debe ser 100.

8. El total de los montos debe ser el monto total del presupuesto mensual.

9. El sistema debe validar que el total de los porcentajes sea 100 y el total de los montos sea el monto total del presupuesto mensual. De lo contrario, no se debe permitir guardar el presupuesto mensual.    

10. Las distribuciones se deben guardar al dar clic en guardar el presupuesto mensual. Si se edita un presupuesto mensual, se deben actualizar las distribuciones correspondientes. Si se elimina un presupuesto mensual, se deben eliminar las distribuciones correspondientes.
11. Las categorias deben ser las siguientes para agregar a la base de datos:
- Obligaciones
- Gustos personales
- Metas-Ahorro
- Imprevistos
- Otros 

12. Mostrar la sesion para la distribucion del presupuesto al cargar el formulario. Para crear o editar, se debe mostrar una distribuccion por defecto para la categoria "Obligaciones" la cual no se puede cambiar en el combobox y solo se puede editar su valor. Por defecto debe asignarse 100 al porcentaje. A partir de ahi se puede crear distribucciones dinamicamente.

--
en el formulario de presupuesto mensual, se debe realizar lo siguiente:

1. El campo "Porcentaje (%)" solo debe permitir numeros enteros entre 0 y 100. Ademas, no debe permitir decimales. No debe permitir valores negativos, ni puntos ni comas ni caracteres especiales. 
2. El campo "Monto" no debe permitir valores negativos, ni puntos ni comas ni caracteres especiales. 
3. No se debe permitir crear o editar distribuciones con la misma categoria. 
4. No se debe permitir crear o editar distribuciones con porcentaje 0 o monto 0.

Al escribir en el campo "Porcentaje (%)" no se debe permitir escribir caracteres especiales, ni puntos ni comas ni espacios. Solo se deben permitir numeros enteros entre 0 y 100.  

--
Al ajustar el saldo del presupuesto mensual, se debe actualizar el monto de la distribucion correspondiente. Por ejemplo, si se aumenta el saldo en 100, se debe aumentar el monto de la distribucion correspondiente en 100. Si se disminuye el saldo en 100, se debe disminuir el monto de la distribucion correspondiente en 100.  

--
Al ajustar el porcentaje de una distribucion, se debe actualizar el monto de la distribucion correspondiente. Por ejemplo, si se aumenta el porcentaje en 10, se debe aumentar el monto de la distribucion correspondiente en 10. Si se disminuye el porcentaje en 10, se debe disminuir el monto de la distribucion correspondiente en 10.  

--
No se debe permitir guardar el presupuesto mensual si ya hay un presupuesto mensual para el mismo año y mes.  

--
En el formulario para crear o editar un gasto, realizar los siguientes cambios:

1. El campo "Monto" no debe permitir valores negativos, ni puntos ni comas ni caracteres especiales. 
2. No se debe permitir crear o editar gastos con monto 0.
3. Se debe agregar un campo para seleccionar la categoria del gasto.    
4. Si se selecciona la categoria "Obligaciones", se debe mostrar un combo box con las siguientes opciones:
- Alimentacion
- Transporte
- Servicios
- Otros

Estas opciones se deben guardar en la base de datos como categorias de obligaciones.    
5. Si se selecciona una categoria diferente a "Obligaciones", se debe ocultar el combo box de categorias de obligaciones

6. El campo "Monto" debe tener un formato de moneda y solo debe abrir el teclado numerico en dispositivos moviles al editarlo.  