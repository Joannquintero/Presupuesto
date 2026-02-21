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
            .Include(s => s.CategoriaPresupuesto)
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
                CategoriaPresupuestoId = dto.CategoriaPresupuestoId,
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
                // 1. Actualizar el Monto Total del presupuesto
                if (dto.Tipo == TipoMovimiento.Agregar)
                    presupuesto.Monto += dto.Monto;
                else
                    presupuesto.Monto -= dto.Monto;

                // 2. Aplicar el cambio a la categoría correspondiente
                var distribucion = presupuesto.Distribuciones.FirstOrDefault(d => d.CategoriaPresupuestoId == dto.CategoriaPresupuestoId);
                if (distribucion != null)
                {
                    if (dto.Tipo == TipoMovimiento.Agregar)
                        distribucion.Monto += dto.Monto;
                    else
                        distribucion.Monto -= dto.Monto;
                }

                // 3. Recalcular los porcentajes de todas las categorías basado en el nuevo total
                foreach (var dist in presupuesto.Distribuciones)
                {
                    if (presupuesto.Monto > 0)
                    {
                        // Usamos decimal para mayor precisión antes de redondear (opcionalmente a 0 decimales si la regla es estricta)
                        dist.Porcentaje = Math.Round((dist.Monto / presupuesto.Monto) * 100, 2);
                    }
                    else
                    {
                        dist.Porcentaje = 0;
                    }
                }
            }

            await _context.SaveChangesAsync();
            
            // Recargar para incluir navegación de categoría
            await _context.Entry(saldo).Reference(s => s.CategoriaPresupuesto).LoadAsync();
            
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

                // Revertir de la categoría correspondiente
                var distribucion = presupuesto.Distribuciones.FirstOrDefault(d => d.CategoriaPresupuestoId == saldo.CategoriaPresupuestoId);
                if (distribucion != null)
                {
                    if (saldo.Tipo == TipoMovimiento.Agregar)
                        distribucion.Monto -= saldo.Monto;
                    else
                        distribucion.Monto += saldo.Monto;
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
        CategoriaPresupuestoId = s.CategoriaPresupuestoId,
        CategoriaNombre = s.CategoriaPresupuesto?.Nombre,
        Monto = s.Monto,
        Concepto = s.Concepto,
        Tipo = s.Tipo,
        Fecha = s.Fecha
    };
}
