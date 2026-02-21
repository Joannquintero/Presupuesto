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
        var saldo = new SaldoPresupuesto
        {
            PresupuestoMensualId = presupuestoId,
            Monto   = dto.Monto,
            Concepto = dto.Concepto,
            Tipo    = dto.Tipo,
            Fecha   = DateTime.Now
        };

        _saldos.Add(saldo);
        await _context.SaveChangesAsync();

        return MapToDto(saldo);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var saldo = await _saldos.FindAsync(id);
        if (saldo is null) return false;

        _saldos.Remove(saldo);
        await _context.SaveChangesAsync();
        return true;
    }

    private static SaldoPresupuestoDto MapToDto(SaldoPresupuesto s) => new()
    {
        Id                    = s.Id,
        PresupuestoMensualId  = s.PresupuestoMensualId,
        Monto                 = s.Monto,
        Concepto              = s.Concepto,
        Tipo                  = s.Tipo,
        Fecha                 = s.Fecha
    };
}
