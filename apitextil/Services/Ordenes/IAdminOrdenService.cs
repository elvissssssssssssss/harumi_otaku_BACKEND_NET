namespace Apitextil.Services.Ordenes;

using Apitextil.DTOs.Ordenes;

public interface IAdminOrdenService
{
    Task CambiarEstadoAsync(long adminUsuarioId, long ordenId, AdminCambiarEstadoOrdenDto dto);
}
