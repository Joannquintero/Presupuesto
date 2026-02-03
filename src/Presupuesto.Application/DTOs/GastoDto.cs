using Presupuesto.Domain.Enums;

namespace Presupuesto.Application.DTOs;

/// <summary>
/// DTO para lectura de gastos.
/// </summary>
public class GastoDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public Categoria Categoria { get; set; }
    public string CategoriaTexto => Categoria.ToString();
    public string Descripcion { get; set; } = string.Empty;
    public decimal Monto { get; set; }
}
