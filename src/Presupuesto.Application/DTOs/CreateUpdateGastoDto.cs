using System.ComponentModel.DataAnnotations;
using Presupuesto.Domain.Enums;

namespace Presupuesto.Application.DTOs;

/// <summary>
/// DTO para crear/actualizar gastos.
/// </summary>
public class CreateUpdateGastoDto
{
    [Required(ErrorMessage = "La fecha es obligatoria")]
    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "La categoría del presupuesto es obligatoria")]
    public int CategoriaPresupuestoId { get; set; }

    public Categoria? SubCategoria { get; set; }

    [StringLength(200, ErrorMessage = "La descripción no puede exceder los 200 caracteres")]
    public string? Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El monto es obligatorio")]
    [Range(0.01, 999999999.99, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Monto { get; set; }
}
