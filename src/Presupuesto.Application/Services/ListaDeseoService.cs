using Microsoft.EntityFrameworkCore;
using Presupuesto.Application.DTOs;
using Presupuesto.Domain.Entities;

namespace Presupuesto.Application.Services;

public class ListaDeseoService : IListaDeseoService
{
    private readonly DbContext _context;
    private readonly DbSet<ListaDeseo> _deseos;

    public ListaDeseoService(DbContext context)
    {
        _context = context;
        _deseos = context.Set<ListaDeseo>();
    }

    public async Task<IEnumerable<ListaDeseoDto>> GetAllAsync()
    {
        return await _deseos
            .OrderByDescending(d => d.Prioridad)
            .ThenBy(d => d.Fecha)
            .Select(d => MapToDto(d))
            .ToListAsync();
    }

    public async Task<ListaDeseoDto?> GetByIdAsync(int id)
    {
        var entity = await _deseos.FirstOrDefaultAsync(d => d.Id == id);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<ListaDeseoDto> CreateAsync(CreateUpdateListaDeseoDto dto)
    {
        var entity = new ListaDeseo
        {
            Concepto = dto.Concepto,
            Monto = dto.Monto,
            Fecha = dto.Fecha,
            Categoria = dto.Categoria,
            Prioridad = dto.Prioridad,
            Activo = dto.Activo
        };

        _deseos.Add(entity);
        await _context.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task UpdateAsync(int id, CreateUpdateListaDeseoDto dto)
    {
        var entity = await _deseos.FindAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"No se encontró el deseo con id {id}");

        entity.Concepto = dto.Concepto;
        entity.Monto = dto.Monto;
        entity.Fecha = dto.Fecha;
        entity.Categoria = dto.Categoria;
        entity.Prioridad = dto.Prioridad;
        entity.Activo = dto.Activo;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _deseos.FindAsync(id);
        if (entity != null)
        {
            _deseos.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    private static ListaDeseoDto MapToDto(ListaDeseo d) => new()
    {
        Id = d.Id,
        Concepto = d.Concepto,
        Monto = d.Monto,
        Fecha = d.Fecha,
        Categoria = d.Categoria,
        Prioridad = d.Prioridad,
        Activo = d.Activo
    };
}
