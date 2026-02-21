using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Presupuesto.Application.DTOs;
using Presupuesto.Domain.Entities;

namespace Presupuesto.Application.Services;

/// <summary>
/// Implementación del servicio de presupuesto mensual.
/// </summary>
public class PresupuestoMensualService : IPresupuestoMensualService
{
    private readonly DbContext _context;
    private readonly DbSet<PresupuestoMensual> _presupuestos;

    public PresupuestoMensualService(DbContext context)
    {
        _context = context;
        _presupuestos = context.Set<PresupuestoMensual>();
    }

    public async Task<List<PresupuestoMensualDto>> GetAllAsync()
    {
        return await _presupuestos
            .OrderByDescending(p => p.Anio)
            .ThenByDescending(p => p.Mes)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<List<PresupuestoMensualDto>> GetByFilterAsync(int? anio, int? mes, string? busqueda)
    {
        var query = _presupuestos.AsQueryable();

        if (anio.HasValue)
            query = query.Where(p => p.Anio == anio.Value);

        if (mes.HasValue)
            query = query.Where(p => p.Mes == mes.Value);

        if (!string.IsNullOrWhiteSpace(busqueda))
            query = query.Where(p => p.Concepto != null && p.Concepto.Contains(busqueda));

        return await query
            .OrderByDescending(p => p.Anio)
            .ThenByDescending(p => p.Mes)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<PresupuestoMensualDto?> GetByIdAsync(int id)
    {
        var presupuesto = await _presupuestos.FindAsync(id);
        return presupuesto is null ? null : MapToDto(presupuesto);
    }

    public async Task<PresupuestoMensualDto> CreateAsync(CreateUpdatePresupuestoMensualDto dto)
    {
        var presupuesto = new PresupuestoMensual
        {
            Anio = dto.Anio,
            Mes = dto.Mes,
            Monto = dto.Monto,
            Concepto = dto.Concepto,
            FechaInicio = new DateTime(dto.Anio, dto.Mes, 1),
            FechaFin = new DateTime(dto.Anio, dto.Mes, 1).AddMonths(1).AddDays(-1)
        };

        _presupuestos.Add(presupuesto);
        await _context.SaveChangesAsync();

        return MapToDto(presupuesto);
    }

    public async Task<PresupuestoMensualDto?> UpdateAsync(int id, CreateUpdatePresupuestoMensualDto dto)
    {
        var presupuesto = await _presupuestos.FindAsync(id);
        if (presupuesto is null) return null;

        presupuesto.Anio = dto.Anio;
        presupuesto.Mes = dto.Mes;
        presupuesto.Monto = dto.Monto;
        presupuesto.Concepto = dto.Concepto;
        presupuesto.FechaInicio = new DateTime(dto.Anio, dto.Mes, 1);
        presupuesto.FechaFin = new DateTime(dto.Anio, dto.Mes, 1).AddMonths(1).AddDays(-1);

        await _context.SaveChangesAsync();
        return MapToDto(presupuesto);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var presupuesto = await _presupuestos.FindAsync(id);
        if (presupuesto is null) return false;

        _presupuestos.Remove(presupuesto);
        await _context.SaveChangesAsync();
        return true;
    }

    private static PresupuestoMensualDto MapToDto(PresupuestoMensual p) => new()
    {
        Id = p.Id,
        Anio = p.Anio,
        Mes = p.Mes,
        Monto = p.Monto,
        Concepto = p.Concepto
    };
}
