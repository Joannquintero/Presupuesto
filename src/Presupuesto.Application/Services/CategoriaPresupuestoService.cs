using Microsoft.EntityFrameworkCore;
using Presupuesto.Domain.Entities;

namespace Presupuesto.Application.Services;

public class CategoriaPresupuestoService : ICategoriaPresupuestoService
{
    private readonly DbContext _context;
    private readonly DbSet<CategoriaPresupuesto> _categorias;

    public CategoriaPresupuestoService(DbContext context)
    {
        _context = context;
        _categorias = context.Set<CategoriaPresupuesto>();
    }

    public async Task<List<CategoriaPresupuesto>> GetAllAsync()
    {
        return await _categorias.ToListAsync();
    }
}
