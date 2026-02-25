using Presupuesto.Application.DTOs;
using Presupuesto.Domain.Enums;

namespace Presupuesto.Application.Services;

/// <summary>
/// Interfaz del servicio de gastos.
/// </summary>
public interface IGastoService
{
    Task<List<GastoDto>> GetAllAsync();
    Task<List<GastoDto>> GetByFilterAsync(int? anio, int? mes, Categoria? categoria);
    Task<GastoDto?> GetByIdAsync(int id);
    Task<GastoDto> CreateAsync(CreateUpdateGastoDto dto);
    Task<GastoDto?> UpdateAsync(int id, CreateUpdateGastoDto dto);
    Task<bool> DeleteAsync(int id);
    Task<ResumenMensualDto> GetResumenMensualAsync(int anio, int mes);
    Task<decimal> GetTotalMesAsync(int anio, int mes);
    Task<decimal?> GetDisponibleCategoriaAsync(int anio, int mes, int categoriaId, int? excludeGastoId = null);
}
