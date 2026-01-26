using Apitextil.DTOs.Ordenes;

namespace Apitextil.Services.Ordenes;

public interface IOrdenService
{
    Task<long> CreateFromCarritoAsync(long usuarioId, CreateOrdenDto dto);
    Task<List<OrdenDto>> GetAllByUsuarioAsync(long usuarioId);
    Task<OrdenDto?> GetByIdAsync(long usuarioId, long ordenId);
    Task<List<OrdenEstadoHistorialDto>> GetHistorialAsync(long usuarioId, long ordenId);
}
