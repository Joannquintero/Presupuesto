using System.ComponentModel.DataAnnotations;
using Presupuesto.Domain.Enums;

namespace Presupuesto.Application.DTOs;

/// <summary>
/// DTO para crear un movimiento de saldo en un presupuesto mensual.
/// </summary>
public class CreateSaldoPresupuestoDto
{
    [Required(ErrorMessage = "La categoría es obligatoria")]
    public int CategoriaPresupuestoId { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio")]
    [Range(0.01, 999999999.99, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Monto { get; set; }

    [StringLength(200, ErrorMessage = "El concepto no puede superar los 200 caracteres")]
    public string? Concepto { get; set; }

    [Required(ErrorMessage = "El tipo es obligatorio")]
    [Range(1, 2, ErrorMessage = "Seleccione un tipo válido")]
    public TipoMovimiento Tipo { get; set; } = TipoMovimiento.Agregar;
}
