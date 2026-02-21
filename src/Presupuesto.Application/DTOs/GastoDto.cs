using Presupuesto.Domain.Enums;

namespace Presupuesto.Application.DTOs;

/// <summary>
/// DTO para lectura de gastos.
/// </summary>
public class GastoDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int CategoriaPresupuestoId { get; set; }
    public string? CategoriaPresupuestoNombre { get; set; }
    public Categoria? SubCategoria { get; set; }
    public string CategoriaTexto => SubCategoria.HasValue ? SubCategoria.Value.ToString() : (CategoriaPresupuestoNombre ?? string.Empty);
    public string Descripcion { get; set; } = string.Empty;
    public decimal Monto { get; set; }
}
