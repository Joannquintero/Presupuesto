using Presupuesto.Domain.Entities;

namespace Presupuesto.Application.Services;

public interface ICategoriaPresupuestoService
{
    Task<List<CategoriaPresupuesto>> GetAllAsync();
    Task<CategoriaPresupuesto?> GetByIdAsync(int id);
    Task<CategoriaPresupuesto> CreateAsync(DTOs.CreateUpdateCategoriaPresupuestoDto dto);
    Task<CategoriaPresupuesto?> UpdateAsync(int id, DTOs.CreateUpdateCategoriaPresupuestoDto dto);
    Task<bool> DeleteAsync(int id);
}
