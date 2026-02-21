using Presupuesto.Domain.Enums;

namespace Presupuesto.Application.DTOs;

/// <summary>
/// DTO de lectura de un movimiento de saldo de presupuesto mensual.
/// </summary>
public class SaldoPresupuestoDto
{
    public int Id { get; set; }
    public int PresupuestoMensualId { get; set; }
    public decimal Monto { get; set; }
    public string? Concepto { get; set; }
    public TipoMovimiento Tipo { get; set; }
    public string TipoTexto => Tipo == TipoMovimiento.Agregar ? "Agregar" : "Quitar";
    public DateTime Fecha { get; set; }
}
