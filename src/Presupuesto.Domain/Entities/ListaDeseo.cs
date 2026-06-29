using System;

namespace Presupuesto.Domain.Entities;

public class ListaDeseo
{
    public int Id { get; set; }
    public string? Concepto { get; set; }
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    
    public int CategoriaPresupuestoId { get; set; }
    public CategoriaPresupuesto CategoriaPresupuesto { get; set; } = null!;
    
    public int Prioridad { get; set; } // 1 a 10
    public bool Activo { get; set; } = true;
}
