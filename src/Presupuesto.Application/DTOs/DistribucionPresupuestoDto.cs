namespace Presupuesto.Application.DTOs;

public class DistribucionPresupuestoDto
{
    public int Id { get; set; }
    public int CategoriaPresupuestoId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public decimal Porcentaje { get; set; }
    public decimal Monto { get; set; }
}
