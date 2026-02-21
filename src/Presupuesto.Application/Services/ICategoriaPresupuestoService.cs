using Presupuesto.Domain.Entities;

namespace Presupuesto.Application.Services;

public interface ICategoriaPresupuestoService
{
    Task<List<CategoriaPresupuesto>> GetAllAsync();
}
