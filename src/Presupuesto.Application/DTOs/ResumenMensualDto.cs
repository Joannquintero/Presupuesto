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

/// <summary>
/// DTO para totales por categoría.
/// </summary>
public class GastoPorCategoriaDto
{
    public Categoria Categoria { get; set; }
    public string CategoriaTexto => Categoria.ToString();
    public decimal Total { get; set; }
    public int Cantidad { get; set; }
}
