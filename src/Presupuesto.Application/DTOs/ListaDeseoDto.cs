using Presupuesto.Domain.Enums;

namespace Presupuesto.Application.DTOs;

public class ListaDeseoDto
{
    public int Id { get; set; }
    public string? Concepto { get; set; }
    public decimal Monto { get; set; }
    public DateTime? Fecha { get; set; }
    public CategoriaDeseo Categoria { get; set; }
    public int Prioridad { get; set; }
    public bool Activo { get; set; }
}
