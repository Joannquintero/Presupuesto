using System.ComponentModel.DataAnnotations;
using Presupuesto.Domain.Enums;

namespace Presupuesto.Application.DTOs;

public class CreateUpdateListaDeseoDto
{
    public int Id { get; set; }

    [StringLength(200, ErrorMessage = "El concepto no puede exceder los 200 caracteres")]
    public string? Concepto { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio")]
    [Range(0, 999999999.99, ErrorMessage = "El monto no puede ser negativo")]
    public decimal Monto { get; set; }

    public DateTime? Fecha { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria")]
    public CategoriaDeseo Categoria { get; set; } = CategoriaDeseo.Otros;

    [Required(ErrorMessage = "La prioridad es obligatoria")]
    [Range(1, 10, ErrorMessage = "La prioridad debe estar entre 1 y 10")]
    public int Prioridad { get; set; } = 5;

    public bool Activo { get; set; } = true;
}
