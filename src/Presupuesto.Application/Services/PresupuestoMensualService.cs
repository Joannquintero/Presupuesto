using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Presupuesto.Application.DTOs;
using Presupuesto.Domain.Entities;
using Presupuesto.Domain.Enums;

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
        var list = await _presupuestos
            .Include(p => p.Saldos)
            .Include(p => p.Distribuciones)
                .ThenInclude(d => d.CategoriaPresupuesto)
            .OrderByDescending(p => p.Anio)
            .ThenByDescending(p => p.Mes)
            .ToListAsync();

        return list.Select(MapToDto).ToList();
    }

    public async Task<List<PresupuestoMensualDto>> GetByFilterAsync(int? anio, int? mes, string? busqueda)
    {
        var query = _presupuestos
            .Include(p => p.Saldos)
            .Include(p => p.Distribuciones)
                .ThenInclude(d => d.CategoriaPresupuesto)
            .AsQueryable();

        if (anio.HasValue)
            query = query.Where(p => p.Anio == anio.Value);

        if (mes.HasValue)
            query = query.Where(p => p.Mes == mes.Value);

        if (!string.IsNullOrWhiteSpace(busqueda))
            query = query.Where(p => p.Concepto != null && p.Concepto.Contains(busqueda));

        var list = await query
            .OrderByDescending(p => p.Anio)
            .ThenByDescending(p => p.Mes)
            .ToListAsync();

        return list.Select(MapToDto).ToList();
    }

    public async Task<PresupuestoMensualDto?> GetByIdAsync(int id)
    {
        var presupuesto = await _presupuestos
            .Include(p => p.Saldos)
            .Include(p => p.Distribuciones)
                .ThenInclude(d => d.CategoriaPresupuesto)
            .FirstOrDefaultAsync(p => p.Id == id);
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
            FechaFin = new DateTime(dto.Anio, dto.Mes, 1).AddMonths(1).AddDays(-1),
            Distribuciones = dto.Distribuciones.Select(d => new DistribucionPresupuesto
            {
                CategoriaPresupuestoId = d.CategoriaPresupuestoId,
                Porcentaje = d.Porcentaje,
                Monto = d.Monto
            }).ToList()
        };

        _presupuestos.Add(presupuesto);
        await _context.SaveChangesAsync();

        return MapToDto(presupuesto);
    }

    public async Task<PresupuestoMensualDto?> UpdateAsync(int id, CreateUpdatePresupuestoMensualDto dto)
    {
        var presupuesto = await _presupuestos
            .Include(p => p.Distribuciones)
            .FirstOrDefaultAsync(p => p.Id == id);
            
        if (presupuesto is null) return null;

        presupuesto.Anio = dto.Anio;
        presupuesto.Mes = dto.Mes;
        presupuesto.Monto = dto.Monto;
        presupuesto.Concepto = dto.Concepto;
        presupuesto.FechaInicio = new DateTime(dto.Anio, dto.Mes, 1);
        presupuesto.FechaFin = new DateTime(dto.Anio, dto.Mes, 1).AddMonths(1).AddDays(-1);

        // Actualizar distribuciones
        presupuesto.Distribuciones.Clear();
        foreach (var d in dto.Distribuciones)
        {
            presupuesto.Distribuciones.Add(new DistribucionPresupuesto
            {
                CategoriaPresupuestoId = d.CategoriaPresupuestoId,
                Porcentaje = d.Porcentaje,
                Monto = d.Monto
            });
        }

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

    private static PresupuestoMensualDto MapToDto(PresupuestoMensual p)
    {
        var agregar = p.Saldos.Where(s => s.Tipo == TipoMovimiento.Agregar).Sum(s => s.Monto);
        var quitar  = p.Saldos.Where(s => s.Tipo == TipoMovimiento.Quitar).Sum(s => s.Monto);

        return new()
        {
            Id          = p.Id,
            Anio        = p.Anio,
            Mes         = p.Mes,
            Monto       = p.Monto,
            Concepto    = p.Concepto,
            SaldoActual = p.Monto,
            Distribuciones = p.Distribuciones.Select(d => new DistribucionPresupuestoDto
            {
                Id = d.Id,
                CategoriaPresupuestoId = d.CategoriaPresupuestoId,
                CategoriaNombre = d.CategoriaPresupuesto?.Nombre ?? string.Empty,
                Porcentaje = d.Porcentaje,
                Monto = d.Monto
            }).ToList()
        };
    }
}
