using Presupuesto.Application.DTOs;

namespace Presupuesto.Application.Services;

/// <summary>
/// Interfaz del servicio de movimientos de saldo de presupuesto mensual.
/// </summary>
public interface ISaldoPresupuestoService
{
    Task<List<SaldoPresupuestoDto>> GetByPresupuestoIdAsync(int presupuestoId);
    Task<SaldoPresupuestoDto> CreateAsync(int presupuestoId, CreateSaldoPresupuestoDto dto);
    Task<bool> DeleteAsync(int id);
}
