namespace Presupuesto.Domain.Entities;

public class CategoriaPresupuesto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool EsSistema { get; set; } = false;
}
