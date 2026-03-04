using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Application.DTOs;

public class CreateUpdateCategoriaPresupuestoDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;
}
