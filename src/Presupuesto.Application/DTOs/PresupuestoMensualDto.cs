using System.Globalization;

namespace Presupuesto.Application.DTOs;

/// <summary>
/// DTO para lectura de presupuesto mensual.
/// </summary>
public class PresupuestoMensualDto
{
    public int Id { get; set; }
    public int Anio { get; set; }
    public int Mes { get; set; }
    public string MesNombre => new DateTime(Anio, Mes, 1).ToString("MMMM", new CultureInfo("es-ES"));
    public decimal Monto { get; set; }
    public string? Concepto { get; set; }
}
