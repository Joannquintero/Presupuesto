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
            .Include(g => g.CategoriaPresupuesto)
            .OrderByDescending(g => g.Fecha)
            .Select(g => MapToDto(g))
            .ToListAsync();
    }

    public async Task<List<GastoDto>> GetByFilterAsync(int? anio, int? mes, Categoria? subCategoria)
    {
        var query = _gastos.Include(g => g.CategoriaPresupuesto).AsQueryable();

        if (anio.HasValue)
            query = query.Where(g => g.Fecha.Year == anio.Value);

        if (mes.HasValue)
            query = query.Where(g => g.Fecha.Month == mes.Value);

        if (subCategoria.HasValue)
            query = query.Where(g => g.SubCategoria == subCategoria.Value);

        return await query
            .OrderByDescending(g => g.Fecha)
            .Select(g => MapToDto(g))
            .ToListAsync();
    }

    public async Task<GastoDto?> GetByIdAsync(int id)
    {
        var gasto = await _gastos
            .Include(g => g.CategoriaPresupuesto)
            .FirstOrDefaultAsync(g => g.Id == id);
        return gasto is null ? null : MapToDto(gasto);
    }

    public async Task<GastoDto> CreateAsync(CreateUpdateGastoDto dto)
    {
        await ValidarPresupuestoYDisponibilidad(dto);

        var gasto = new Gasto
        {
            Fecha = dto.Fecha,
            CategoriaPresupuestoId = dto.CategoriaPresupuestoId,
            SubCategoria = dto.SubCategoria,
            Descripcion = dto.Descripcion,
            Monto = dto.Monto
        };

        _gastos.Add(gasto);
        await _context.SaveChangesAsync();

        // Cargar navegación para el MapToDto
        await _context.Entry(gasto).Reference(g => g.CategoriaPresupuesto).LoadAsync();

        return MapToDto(gasto);
    }

    public async Task<GastoDto?> UpdateAsync(int id, CreateUpdateGastoDto dto)
    {
        await ValidarPresupuestoYDisponibilidad(dto, id);

        var gasto = await _gastos.FindAsync(id);
        if (gasto is null) return null;

        gasto.Fecha = dto.Fecha;
        gasto.CategoriaPresupuestoId = dto.CategoriaPresupuestoId;
        gasto.SubCategoria = dto.SubCategoria;
        gasto.Descripcion = dto.Descripcion;
        gasto.Monto = dto.Monto;

        await _context.SaveChangesAsync();
        await _context.Entry(gasto).Reference(g => g.CategoriaPresupuesto).LoadAsync();
        
        return MapToDto(gasto);
    }

    private async Task ValidarPresupuestoYDisponibilidad(CreateUpdateGastoDto dto, int? excludeId = null)
    {
        int anio = dto.Fecha.Year;
        int mes = dto.Fecha.Month;

        // 1. Validar que exista un presupuesto mensual para el periodo
        var presupuesto = await _context.Set<PresupuestoMensual>()
            .Include(p => p.Distribuciones)
            .FirstOrDefaultAsync(p => p.Anio == anio && p.Mes == mes);

        if (presupuesto == null)
        {
            throw new InvalidOperationException($"No existe un presupuesto mensual configurado para {anio}-{mes:D2}. Debe crearlo antes de registrar gastos.");
        }

        // 2. Validar disponibilidad en la categoría
        var distribucion = presupuesto.Distribuciones
            .FirstOrDefault(d => d.CategoriaPresupuestoId == dto.CategoriaPresupuestoId);

        if (distribucion == null)
        {
            throw new InvalidOperationException("La categoría seleccionada no tiene una distribución asignada en el presupuesto de este mes.");
        }

        // Calcular gastos actuales en esta categoría para este mes (excluyendo el actual si es edición)
        var gastosActualesDouble = await _gastos
            .Where(g => g.Fecha.Year == anio && 
                        g.Fecha.Month == mes && 
                        g.CategoriaPresupuestoId == dto.CategoriaPresupuestoId &&
                        (!excludeId.HasValue || g.Id != excludeId.Value))
            .SumAsync(g => (double)g.Monto);

        var gastosActuales = (decimal)gastosActualesDouble;

        decimal disponible = distribucion.Monto - gastosActuales;

        if (dto.Monto > disponible)
        {
            throw new InvalidOperationException($"El monto del gasto (${dto.Monto:N0}) excede el presupuesto disponible para esta categoría (${disponible:N0}).");
        }
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
            .Include(g => g.CategoriaPresupuesto)
            .Where(g => g.Fecha.Year == anio && g.Fecha.Month == mes)
            .ToListAsync();

        var resumen = new ResumenMensualDto
        {
            Anio = anio,
            Mes = mes,
            MesNombre = new DateTime(anio, mes, 1).ToString("MMMM", new CultureInfo("es-ES")),
            TotalMes = (decimal)gastosMes.Sum(g => (double)g.Monto),
            GastosPorCategoria = gastosMes
                .GroupBy(g => g.CategoriaPresupuestoId)
                .Select(group => new GastoPorCategoriaDto
                {
                    CategoriaPresupuestoId = group.Key,
                    CategoriaNombre = group.First().CategoriaPresupuesto?.Nombre ?? "Sin Categoría",
                    Total = (decimal)group.Sum(g => (double)g.Monto),
                    Cantidad = group.Count()
                })
                .OrderByDescending(x => x.Total)
                .ToList()
        };

        return resumen;
    }

    public async Task<decimal> GetTotalMesAsync(int anio, int mes)
    {
        var totalDouble = await _gastos
            .Where(g => g.Fecha.Year == anio && g.Fecha.Month == mes)
            .SumAsync(g => (double)g.Monto);
        
        return (decimal)totalDouble;
    }

    public async Task<decimal?> GetDisponibleCategoriaAsync(int anio, int mes, int categoriaId, int? excludeGastoId = null)
    {
        var presupuesto = await _context.Set<PresupuestoMensual>()
            .Include(p => p.Distribuciones)
            .FirstOrDefaultAsync(p => p.Anio == anio && p.Mes == mes);

        if (presupuesto == null) return null;

        var distribucion = presupuesto.Distribuciones
            .FirstOrDefault(d => d.CategoriaPresupuestoId == categoriaId);

        if (distribucion == null) return null;

        var gastosActualesDouble = await _gastos
            .Where(g => g.Fecha.Year == anio && 
                        g.Fecha.Month == mes && 
                        g.CategoriaPresupuestoId == categoriaId &&
                        (!excludeGastoId.HasValue || g.Id != excludeGastoId.Value))
            .SumAsync(g => (double)g.Monto);

        var gastosActuales = (decimal)gastosActualesDouble;

        return distribucion.Monto - gastosActuales;
    }

    private static GastoDto MapToDto(Gasto gasto) => new()
    {
        Id = gasto.Id,
        Fecha = gasto.Fecha,
        CategoriaPresupuestoId = gasto.CategoriaPresupuestoId,
        CategoriaPresupuestoNombre = gasto.CategoriaPresupuesto?.Nombre,
        SubCategoria = gasto.SubCategoria,
        Descripcion = gasto.Descripcion,
        Monto = gasto.Monto
    };
}
