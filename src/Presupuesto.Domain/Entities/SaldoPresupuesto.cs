using System.ComponentModel.DataAnnotations;
using Presupuesto.Domain.Enums;

namespace Presupuesto.Domain.Entities;

/// <summary>
/// Representa un movimiento de ajuste de saldo sobre un presupuesto mensual.
/// </summary>
public class SaldoPresupuesto
{
    public int Id { get; set; }

    [Required]
    public int PresupuestoMensualId { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio")]
    [Range(0.01, 999999999.99, ErrorMessage = "El monto debe ser mayor a 0")]
    [DataType(DataType.Currency)]
    public decimal Monto { get; set; }

    [StringLength(200, ErrorMessage = "El concepto no puede superar los 200 caracteres")]
    public string? Concepto { get; set; }

    [Required(ErrorMessage = "El tipo es obligatorio")]
    public TipoMovimiento Tipo { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    // Navegación
    public PresupuestoMensual PresupuestoMensual { get; set; } = null!;
}
