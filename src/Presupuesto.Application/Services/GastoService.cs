using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Presupuesto.Application.DTOs;
using Presupuesto.Domain.Entities;
using Presupuesto.Domain.Enums;

namespace Presupuesto.Application.Services;

/// <summary>
/// Implementación del servicio de gastos.
/// </summary>
public class GastoService : IGastoService
{
    private readonly DbContext _context;
    private readonly DbSet<Gasto> _gastos;

    public GastoService(DbContext context)
    {
        _context = context;
        _gastos = context.Set<Gasto>();
    }

    public async Task<List<GastoDto>> GetAllAsync()
    {
        return await _gastos
            .OrderByDescending(g => g.Fecha)
            .Select(g => MapToDto(g))
            .ToListAsync();
    }

    public async Task<List<GastoDto>> GetByFilterAsync(int? anio, int? mes, Categoria? categoria)
    {
        var query = _gastos.AsQueryable();

        if (anio.HasValue)
            query = query.Where(g => g.Fecha.Year == anio.Value);

        if (mes.HasValue)
            query = query.Where(g => g.Fecha.Month == mes.Value);

        if (categoria.HasValue)
            query = query.Where(g => g.Categoria == categoria.Value);

        return await query
            .OrderByDescending(g => g.Fecha)
            .Select(g => MapToDto(g))
            .ToListAsync();
    }

    public async Task<GastoDto?> GetByIdAsync(int id)
    {
        var gasto = await _gastos.FindAsync(id);
        return gasto is null ? null : MapToDto(gasto);
    }

    public async Task<GastoDto> CreateAsync(CreateUpdateGastoDto dto)
    {
        var gasto = new Gasto
        {
            Fecha = dto.Fecha,
            Categoria = dto.Categoria,
            Descripcion = dto.Descripcion,
            Monto = dto.Monto
        };

        _gastos.Add(gasto);
        await _context.SaveChangesAsync();

        return MapToDto(gasto);
    }

    public async Task<GastoDto?> UpdateAsync(int id, CreateUpdateGastoDto dto)
    {
        var gasto = await _gastos.FindAsync(id);
        if (gasto is null) return null;

        gasto.Fecha = dto.Fecha;
        gasto.Categoria = dto.Categoria;
        gasto.Descripcion = dto.Descripcion;
        gasto.Monto = dto.Monto;

        await _context.SaveChangesAsync();
        return MapToDto(gasto);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var gasto = await _gastos.FindAsync(id);
        if (gasto is null) return false;

        _gastos.Remove(gasto);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ResumenMensualDto> GetResumenMensualAsync(int anio, int mes)
    {
        var gastosMes = await _gastos
            .Where(g => g.Fecha.Year == anio && g.Fecha.Month == mes)
            .ToListAsync();

        var resumen = new ResumenMensualDto
        {
            Anio = anio,
            Mes = mes,
            MesNombre = new DateTime(anio, mes, 1).ToString("MMMM", new CultureInfo("es-ES")),
            TotalMes = gastosMes.Sum(g => g.Monto),
            GastosPorCategoria = gastosMes
                .GroupBy(g => g.Categoria)
                .Select(group => new GastoPorCategoriaDto
                {
                    Categoria = group.Key,
                    Total = group.Sum(g => g.Monto),
                    Cantidad = group.Count()
                })
                .OrderByDescending(x => x.Total)
                .ToList()
        };

        return resumen;
    }

    public async Task<decimal> GetTotalMesAsync(int anio, int mes)
    {
        return await _gastos
            .Where(g => g.Fecha.Year == anio && g.Fecha.Month == mes)
            .SumAsync(g => g.Monto);
    }

    private static GastoDto MapToDto(Gasto gasto) => new()
    {
        Id = gasto.Id,
        Fecha = gasto.Fecha,
        Categoria = gasto.Categoria,
        Descripcion = gasto.Descripcion,
        Monto = gasto.Monto
    };
}
