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

    [Required(ErrorMessage = "La categoría es obligatoria")]
    public Categoria Categoria { get; set; } = Categoria.Otros;

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "La descripción debe tener entre 3 y 200 caracteres")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El monto es obligatorio")]
    [Range(0.01, 999999.99, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Monto { get; set; }
}
