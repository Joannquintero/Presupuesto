using Presupuesto.Application.DTOs;

namespace Presupuesto.Application.Services;

public interface IGastoFijoService
{
    Task<IEnumerable<GastoFijoDto>> GetAllAsync();
    Task<IEnumerable<GastoFijoDto>> GetActivosAsync();
    Task<GastoFijoDto?> GetByIdAsync(int id);
    Task<GastoFijoDto> CreateAsync(CreateUpdateGastoFijoDto dto);
    Task UpdateAsync(int id, CreateUpdateGastoFijoDto dto);
    Task DeleteAsync(int id);
}
