using System.ComponentModel.DataAnnotations;
using Presupuesto.Domain.Enums;

namespace Presupuesto.Application.DTOs;

public class CreateUpdateGastoFijoDto
{
    public int Id { get; set; }

    [StringLength(200, ErrorMessage = "El concepto no puede exceder los 200 caracteres")]
    public string? Concepto { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio")]
    [Range(0.01, 999999999.99, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Monto { get; set; }

    [Required(ErrorMessage = "La frecuencia es obligatoria")]
    public FrecuenciaGasto Frecuencia { get; set; } = FrecuenciaGasto.Mensual;

    [Required(ErrorMessage = "La fecha es obligatoria")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "La categoría es obligatoria")]
    public int CategoriaPresupuestoId { get; set; }

    public bool Activo { get; set; } = true;
}
