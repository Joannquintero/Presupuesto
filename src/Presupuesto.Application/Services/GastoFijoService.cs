using Microsoft.EntityFrameworkCore;
using Presupuesto.Application.DTOs;
using Presupuesto.Domain.Entities;

namespace Presupuesto.Application.Services;

public class GastoFijoService : IGastoFijoService
{
    private readonly DbContext _context;
    private readonly DbSet<GastoFijoRecurrente> _gastosFijos;

    public GastoFijoService(DbContext context)
    {
        _context = context;
        _gastosFijos = context.Set<GastoFijoRecurrente>();
    }

    public async Task<IEnumerable<GastoFijoDto>> GetAllAsync()
    {
        return await _gastosFijos
            .Include(g => g.CategoriaPresupuesto)
            .OrderBy(g => g.Fecha)
            .Select(g => new GastoFijoDto
            {
                Id = g.Id,
                Concepto = g.Concepto,
                Monto = g.Monto,
                Frecuencia = g.Frecuencia,
                Fecha = g.Fecha,
                CategoriaPresupuestoId = g.CategoriaPresupuestoId,
                CategoriaNombre = g.CategoriaPresupuesto.Nombre,
                Activo = g.Activo
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<GastoFijoDto>> GetActivosAsync()
    {
        return await _gastosFijos
            .Include(g => g.CategoriaPresupuesto)
            .Where(g => g.Activo)
            .OrderBy(g => g.Concepto)
            .Select(g => new GastoFijoDto
            {
                Id = g.Id,
                Concepto = g.Concepto,
                Monto = g.Monto,
                Frecuencia = g.Frecuencia,
                Fecha = g.Fecha,
                CategoriaPresupuestoId = g.CategoriaPresupuestoId,
                CategoriaNombre = g.CategoriaPresupuesto.Nombre,
                Activo = g.Activo
            })
            .ToListAsync();
    }

    public async Task<GastoFijoDto?> GetByIdAsync(int id)
    {
        var g = await _gastosFijos
            .Include(x => x.CategoriaPresupuesto)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (g == null) return null;

        return new GastoFijoDto
        {
            Id = g.Id,
            Concepto = g.Concepto,
            Monto = g.Monto,
            Frecuencia = g.Frecuencia,
            Fecha = g.Fecha,
            CategoriaPresupuestoId = g.CategoriaPresupuestoId,
            CategoriaNombre = g.CategoriaPresupuesto.Nombre,
            Activo = g.Activo
        };
    }

    public async Task<GastoFijoDto> CreateAsync(CreateUpdateGastoFijoDto dto)
    {
        var entity = new GastoFijoRecurrente
        {
            Concepto = dto.Concepto,
            Monto = dto.Monto,
            Frecuencia = dto.Frecuencia,
            Fecha = dto.Fecha,
            CategoriaPresupuestoId = dto.CategoriaPresupuestoId,
            Activo = dto.Activo
        };

        _gastosFijos.Add(entity);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(entity.Id) ?? throw new InvalidOperationException("Error al crear Gasto Fijo");
    }

    public async Task UpdateAsync(int id, CreateUpdateGastoFijoDto dto)
    {
        var entity = await _gastosFijos.FindAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"No se encontró el gasto fijo con id {id}");

        entity.Concepto = dto.Concepto;
        entity.Monto = dto.Monto;
        entity.Frecuencia = dto.Frecuencia;
        entity.Fecha = dto.Fecha;
        entity.CategoriaPresupuestoId = dto.CategoriaPresupuestoId;
        entity.Activo = dto.Activo;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _gastosFijos.FindAsync(id);
        if (entity != null)
        {
            _gastosFijos.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
