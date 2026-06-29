using Presupuesto.Application.DTOs;

namespace Presupuesto.Application.Services;

public interface IListaDeseoService
{
    Task<IEnumerable<ListaDeseoDto>> GetAllAsync();
    Task<ListaDeseoDto?> GetByIdAsync(int id);
    Task<ListaDeseoDto> CreateAsync(CreateUpdateListaDeseoDto dto);
    Task UpdateAsync(int id, CreateUpdateListaDeseoDto dto);
    Task DeleteAsync(int id);
}
