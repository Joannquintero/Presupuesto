using Microsoft.EntityFrameworkCore;
using Presupuesto.Application.DTOs;
using Presupuesto.Domain.Entities;
using Presupuesto.Domain.Enums;

namespace Presupuesto.Application.Services;

/// <summary>
/// Implementación del servicio de movimientos de saldo de presupuesto mensual.
/// </summary>
public class SaldoPresupuestoService : ISaldoPresupuestoService
{
    private readonly DbContext _context;
    private readonly DbSet<SaldoPresupuesto> _saldos;

    public SaldoPresupuestoService(DbContext context)
    {
        _context = context;
        _saldos = context.Set<SaldoPresupuesto>();
    }

    public async Task<List<SaldoPresupuestoDto>> GetByPresupuestoIdAsync(int presupuestoId)
    {
        return await _saldos
            .Where(s => s.PresupuestoMensualId == presupuestoId)
            .OrderByDescending(s => s.Fecha)
            .Select(s => MapToDto(s))
            .ToListAsync();
    }

    public async Task<SaldoPresupuestoDto> CreateAsync(int presupuestoId, CreateSaldoPresupuestoDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var saldo = new SaldoPresupuesto
            {
                PresupuestoMensualId = presupuestoId,
                Monto = dto.Monto,
                Concepto = dto.Concepto,
                Tipo = dto.Tipo,
                Fecha = DateTime.Now
            };

            _saldos.Add(saldo);

            var presupuesto = await _context.Set<PresupuestoMensual>()
                .Include(p => p.Distribuciones)
                .FirstOrDefaultAsync(p => p.Id == presupuestoId);

            if (presupuesto != null)
            {
                // 1. Actualizar el Monto Total
                if (dto.Tipo == TipoMovimiento.Agregar)
                    presupuesto.Monto += dto.Monto;
                else
                    presupuesto.Monto -= dto.Monto;

                // 2. Aplicar el cambio a la categoría "Obligaciones" (Id=1)
                var obligaciones = presupuesto.Distribuciones.FirstOrDefault(d => d.CategoriaPresupuestoId == 1);
                if (obligaciones != null)
                {
                    if (dto.Tipo == TipoMovimiento.Agregar)
                        obligaciones.Monto += dto.Monto;
                    else
                        obligaciones.Monto -= dto.Monto;
                }

                // 3. Recalcular los porcentajes de todas las categorías
                foreach (var dist in presupuesto.Distribuciones)
                {
                    if (presupuesto.Monto > 0)
                        dist.Porcentaje = Math.Round((dist.Monto / presupuesto.Monto) * 100, 2);
                    else
                        dist.Porcentaje = 0;
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return MapToDto(saldo);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var saldo = await _saldos.FindAsync(id);
        if (saldo is null) return false;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var presupuesto = await _context.Set<PresupuestoMensual>()
                .Include(p => p.Distribuciones)
                .FirstOrDefaultAsync(p => p.Id == saldo.PresupuestoMensualId);

            if (presupuesto != null)
            {
                // Revertir el Monto Total
                if (saldo.Tipo == TipoMovimiento.Agregar)
                    presupuesto.Monto -= saldo.Monto;
                else
                    presupuesto.Monto += saldo.Monto;

                // Revertir de "Obligaciones" (Id=1)
                var obligaciones = presupuesto.Distribuciones.FirstOrDefault(d => d.CategoriaPresupuestoId == 1);
                if (obligaciones != null)
                {
                    if (saldo.Tipo == TipoMovimiento.Agregar)
                        obligaciones.Monto -= saldo.Monto;
                    else
                        obligaciones.Monto += saldo.Monto;
                }

                // Recalcular porcentajes
                foreach (var dist in presupuesto.Distribuciones)
                {
                    if (presupuesto.Monto > 0)
                        dist.Porcentaje = Math.Round((dist.Monto / presupuesto.Monto) * 100, 2);
                    else
                        dist.Porcentaje = 0;
                }
            }

            _saldos.Remove(saldo);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static SaldoPresupuestoDto MapToDto(SaldoPresupuesto s) => new()
    {
        Id = s.Id,
        PresupuestoMensualId = s.PresupuestoMensualId,
        Monto = s.Monto,
        Concepto = s.Concepto,
        Tipo = s.Tipo,
        Fecha = s.Fecha
    };
}
