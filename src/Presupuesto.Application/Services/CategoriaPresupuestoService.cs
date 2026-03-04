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

    public async Task<CategoriaPresupuesto?> GetByIdAsync(int id)
    {
        return await _categorias.FindAsync(id);
    }

    public async Task<CategoriaPresupuesto> CreateAsync(DTOs.CreateUpdateCategoriaPresupuestoDto dto)
    {
        var existe = await _categorias.AnyAsync(c => c.Nombre.ToLower() == dto.Nombre.ToLower());
        if (existe) throw new InvalidOperationException("Ya existe una categoría con este nombre.");

        var categoria = new CategoriaPresupuesto
        {
            Nombre = dto.Nombre,
            EsSistema = false
        };

        _categorias.Add(categoria);
        await _context.SaveChangesAsync();
        return categoria;
    }

    public async Task<CategoriaPresupuesto?> UpdateAsync(int id, DTOs.CreateUpdateCategoriaPresupuestoDto dto)
    {
        var categoria = await _categorias.FindAsync(id);
        if (categoria is null) return null;
        if (categoria.EsSistema) throw new InvalidOperationException("No se puede editar una categoría de sistema.");

        var existe = await _categorias.AnyAsync(c => c.Id != id && c.Nombre.ToLower() == dto.Nombre.ToLower());
        if (existe) throw new InvalidOperationException("Ya existe otra categoría con este nombre.");

        categoria.Nombre = dto.Nombre;
        await _context.SaveChangesAsync();
        return categoria;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var categoria = await _categorias.FindAsync(id);
        if (categoria is null) return false;
        if (categoria.EsSistema) throw new InvalidOperationException("No se puede eliminar una categoría de sistema.");

        // Verificar si tiene gastos
        var tieneGastos = await _context.Set<Gasto>().AnyAsync(g => g.CategoriaPresupuestoId == id);
        if (tieneGastos) throw new InvalidOperationException("No se puede eliminar la categoría porque hay gastos asociados a ella.");

        // Verificar si tiene distribuciones en presupuestos
        var tieneDistribuciones = await _context.Set<DistribucionPresupuesto>().AnyAsync(d => d.CategoriaPresupuestoId == id);
        if (tieneDistribuciones) throw new InvalidOperationException("No se puede eliminar la categoría porque está siendo utilizada en distribuciones de presupuesto.");

        _categorias.Remove(categoria);
        await _context.SaveChangesAsync();
        return true;
    }
}
