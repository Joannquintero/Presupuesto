using System.ComponentModel.DataAnnotations;
using Presupuesto.Domain.Enums;

namespace Presupuesto.Application.DTOs;

public class GastoFijoDto
{
    public int Id { get; set; }
    public string? Concepto { get; set; }
    public decimal Monto { get; set; }
    public FrecuenciaGasto Frecuencia { get; set; }
    public DateTime Fecha { get; set; }
    public int CategoriaPresupuestoId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
