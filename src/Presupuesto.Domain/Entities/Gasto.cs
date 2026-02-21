using System.ComponentModel.DataAnnotations;
using Presupuesto.Domain.Enums;

namespace Presupuesto.Domain.Entities;

/// <summary>
/// Entidad que representa un gasto personal.
/// </summary>
public class Gasto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria")]
    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "La categoría del presupuesto es obligatoria")]
    public int CategoriaPresupuestoId { get; set; }

    public Categoria? SubCategoria { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "La descripción debe tener entre 3 y 200 caracteres")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El monto es obligatorio")]
    [Range(0.01, 999999999.99, ErrorMessage = "El monto debe ser mayor a 0")]
    [DataType(DataType.Currency)]
    public decimal Monto { get; set; }

    // Navegación
    public CategoriaPresupuesto CategoriaPresupuesto { get; set; } = null!;
}
