using Apitextil.DTOs.Productos;

namespace Apitextil.Services.Productos;

public interface IProductoService
{
    Task<List<ProductoDto>> GetAllAsync(long? categoriaId = null, bool? activo = null);
    Task<ProductoDto?> GetByIdAsync(long id);

    Task<ProductoDto> CreateAsync(CreateProductoForm form);
    Task<ProductoDto> UpdateAsync(long id, UpdateProductoForm form);
    Task<ProductoDto> SetActivoAsync(long id, bool activo);

}
