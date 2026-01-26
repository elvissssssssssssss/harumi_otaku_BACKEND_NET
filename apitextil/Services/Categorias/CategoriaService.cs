using Apitextil.Data;
using Apitextil.DTOs.Categorias;
using Apitextil.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Apitextil.Services.Categorias;

public class CategoriaService : ICategoriaService
{
    private readonly EcommerceContext _db;

    public CategoriaService(EcommerceContext db) => _db = db;

    public async Task<List<CategoriaDto>> GetAllAsync()
    {
        return await _db.tblcategorias
            .OrderBy(c => c.Nombre)
            .Select(c => new CategoriaDto { Id = c.Id, Nombre = c.Nombre })
            .ToListAsync();
    }

    public async Task<CategoriaDto?> GetByIdAsync(long id)
    {
        return await _db.tblcategorias
            .Where(c => c.Id == id)
            .Select(c => new CategoriaDto { Id = c.Id, Nombre = c.Nombre })
            .FirstOrDefaultAsync();
    }

    public async Task<CategoriaDto> CreateAsync(CreateCategoriaDto dto)
    {
        var nombre = (dto.Nombre ?? "").Trim();
        if (string.IsNullOrWhiteSpace(nombre))
            throw new InvalidOperationException("Nombre requerido.");

        var exists = await _db.tblcategorias.AnyAsync(x => x.Nombre == nombre);
        if (exists) throw new InvalidOperationException("La categoría ya existe.");

        var entity = new tblCategoria
        {
            Nombre = nombre,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.tblcategorias.Add(entity);
        await _db.SaveChangesAsync();

        return new CategoriaDto { Id = entity.Id, Nombre = entity.Nombre };
    }

    public async Task<CategoriaDto> UpdateAsync(long id, UpdateCategoriaDto dto)
    {
        var nombre = (dto.Nombre ?? "").Trim();
        if (string.IsNullOrWhiteSpace(nombre))
            throw new InvalidOperationException("Nombre requerido.");

        var entity = await _db.tblcategorias.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) throw new InvalidOperationException("Categoría no encontrada.");

        var exists = await _db.tblcategorias.AnyAsync(x => x.Nombre == nombre && x.Id != id);
        if (exists) throw new InvalidOperationException("Ya existe otra categoría con ese nombre.");

        entity.Nombre = nombre;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new CategoriaDto { Id = entity.Id, Nombre = entity.Nombre };
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _db.tblcategorias.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return;

        _db.tblcategorias.Remove(entity);
        await _db.SaveChangesAsync();
    }
}
