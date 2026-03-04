using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Domain.Entities;

public class DistribucionPresupuesto
{
    public int Id { get; set; }
    public int PresupuestoMensualId { get; set; }
    public int CategoriaPresupuestoId { get; set; }

    [Required]
    [Range(0, 100)]
    public decimal Porcentaje { get; set; }

    [Required]
    public decimal Monto { get; set; }

    public bool Bloqueada { get; set; } = false;

    // Propiedades de Navegación
    public PresupuestoMensual PresupuestoMensual { get; set; } = null!;
    public CategoriaPresupuesto CategoriaPresupuesto { get; set; } = null!;
}
