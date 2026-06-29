using System;
using Presupuesto.Domain.Enums;

namespace Presupuesto.Domain.Entities;

public class GastoFijoRecurrente
{
    public int Id { get; set; }
    public string? Concepto { get; set; }
    public decimal Monto { get; set; }
    public FrecuenciaGasto Frecuencia { get; set; }
    public DateTime Fecha { get; set; }
    
    public int CategoriaPresupuestoId { get; set; }
    public CategoriaPresupuesto CategoriaPresupuesto { get; set; } = null!;
    
    public bool Activo { get; set; } = true;
}
