using Apitextil.DTOs.Categorias;

namespace Apitextil.Services.Categorias;

public interface ICategoriaService
{
    Task<List<CategoriaDto>> GetAllAsync();
    Task<CategoriaDto?> GetByIdAsync(long id);
    Task<CategoriaDto> CreateAsync(CreateCategoriaDto dto);
    Task<CategoriaDto> UpdateAsync(long id, UpdateCategoriaDto dto);
    Task DeleteAsync(long id);
}
