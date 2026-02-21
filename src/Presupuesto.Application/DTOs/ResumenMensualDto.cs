using Presupuesto.Domain.Enums;

namespace Presupuesto.Application.DTOs;

/// <summary>
/// DTO para el resumen mensual de gastos.
/// </summary>
public class ResumenMensualDto
{
    public int Anio { get; set; }
    public int Mes { get; set; }
    public string MesNombre { get; set; } = string.Empty;
    public decimal TotalMes { get; set; }
    public List<GastoPorCategoriaDto> GastosPorCategoria { get; set; } = new();
}

public class GastoPorCategoriaDto
{
    public int CategoriaPresupuestoId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Cantidad { get; set; }
}
