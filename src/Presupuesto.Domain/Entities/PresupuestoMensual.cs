using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Domain.Entities;

/// <summary>
/// Entidad que representa un presupuesto mensual.
/// </summary>
public class PresupuestoMensual
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El año es obligatorio")]
    [Range(2000, 2100, ErrorMessage = "El año debe ser válido")]
    public int Anio { get; set; }

    [Required(ErrorMessage = "El mes es obligatorio")]
    [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12")]
    public int Mes { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio")]
    [Range(0.01, 999999999.99, ErrorMessage = "El monto debe ser mayor a 0")]
    [DataType(DataType.Currency)]
    public decimal Monto { get; set; }

    [StringLength(200, ErrorMessage = "El concepto no puede superar los 200 caracteres")]
    public string? Concepto { get; set; }

    /// <summary>
    /// Fecha de inicio del mes. Se calcula automáticamente al crear o editar.
    /// No se muestra en la UI.
    /// </summary>
    public DateTime FechaInicio { get; set; }

    /// <summary>
    /// Fecha de fin del mes. Se calcula automáticamente al crear o editar.
    /// No se muestra en la UI.
    /// </summary>
    public DateTime FechaFin { get; set; }

    // Navegación
    public ICollection<SaldoPresupuesto> Saldos { get; set; } = new List<SaldoPresupuesto>();
    public ICollection<DistribucionPresupuesto> Distribuciones { get; set; } = new List<DistribucionPresupuesto>();
}
