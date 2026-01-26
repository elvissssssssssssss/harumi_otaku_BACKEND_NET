using Apitextil.DTOs.Carrito;

namespace Apitextil.Services.Carrito;

public interface ICarritoService
{
    Task<CarritoActualDto> GetCarritoActualAsync(long usuarioId);
    Task<CarritoActualDto> AddItemAsync(long usuarioId, AddCarritoItemDto dto);
    Task<CarritoActualDto> UpdateItemAsync(long usuarioId, long itemId, UpdateCarritoItemDto dto);
    Task<CarritoActualDto> DeleteItemAsync(long usuarioId, long itemId);
}
