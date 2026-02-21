using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Application.DTOs;

/// <summary>
/// DTO para crear o actualizar un presupuesto mensual.
/// </summary>
public class CreateUpdatePresupuestoMensualDto
{
    [Required(ErrorMessage = "El año es obligatorio")]
    [Range(2000, 2100, ErrorMessage = "El año debe ser válido")]
    public int Anio { get; set; }

    [Required(ErrorMessage = "El mes es obligatorio")]
    [Range(1, 12, ErrorMessage = "Seleccione un mes válido")]
    public int Mes { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio")]
    [Range(0.01, 999999999.99, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Monto { get; set; }

    [StringLength(200, ErrorMessage = "El concepto no puede superar los 200 caracteres")]
    public string? Concepto { get; set; }

    public List<DistribucionPresupuestoDto> Distribuciones { get; set; } = new();
}
