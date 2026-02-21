using Presupuesto.Application.DTOs;

namespace Presupuesto.Application.Services;

/// <summary>
/// Interfaz del servicio de presupuesto mensual.
/// </summary>
public interface IPresupuestoMensualService
{
    Task<List<PresupuestoMensualDto>> GetAllAsync();
    Task<List<PresupuestoMensualDto>> GetByFilterAsync(int? anio, int? mes, string? busqueda);
    Task<PresupuestoMensualDto?> GetByIdAsync(int id);
    Task<PresupuestoMensualDto> CreateAsync(CreateUpdatePresupuestoMensualDto dto);
    Task<PresupuestoMensualDto?> UpdateAsync(int id, CreateUpdatePresupuestoMensualDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int anio, int mes, int? excludeId = null);
}
