using Apitextil.Data;
using Apitextil.DTOs.Productos;
using Apitextil.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Apitextil.Services.Productos;

public class ProductoService : IProductoService
{
    private readonly EcommerceContext _db;
    private readonly IWebHostEnvironment _env;

    public ProductoService(EcommerceContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<List<ProductoDto>> GetAllAsync(long? categoriaId = null, bool? activo = null)
    {
        var q = _db.tblproductos.AsQueryable();

        if (categoriaId.HasValue) q = q.Where(x => x.CategoriaId == categoriaId.Value);
        if (activo.HasValue) q = q.Where(x => x.Activo == activo.Value);

        return await q.OrderBy(x => x.Nombre)
            .Select(x => new ProductoDto
            {
                Id = x.Id,
                CategoriaId = x.CategoriaId,
                Nombre = x.Nombre,
                Descripcion = x.Descripcion,
                FotoUrl = x.FotoUrl,
                Precio = x.Precio,
                Activo = x.Activo
            })
            .ToListAsync();
    }

    public async Task<ProductoDto?> GetByIdAsync(long id)
    {
        return await _db.tblproductos
            .Where(x => x.Id == id)
            .Select(x => new ProductoDto
            {
                Id = x.Id,
                CategoriaId = x.CategoriaId,
                Nombre = x.Nombre,
                Descripcion = x.Descripcion,
                FotoUrl = x.FotoUrl,
                Precio = x.Precio,
                Activo = x.Activo
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProductoDto> CreateAsync(CreateProductoForm form)
    {
        var nombre = (form.Nombre ?? "").Trim();
        if (string.IsNullOrWhiteSpace(nombre)) throw new InvalidOperationException("Nombre requerido.");
        if (form.Precio < 0) throw new InvalidOperationException("Precio inválido.");

        var categoriaExists = await _db.tblcategorias.AnyAsync(c => c.Id == form.CategoriaId);
        if (!categoriaExists) throw new InvalidOperationException("Categoría no existe.");

        var fotoUrl = form.Foto != null ? await SaveProductoFotoAsync(form.Foto) : null;

        var entity = new tblProducto
        {
            CategoriaId = form.CategoriaId,
            Nombre = nombre,
            Descripcion = form.Descripcion,
            Precio = form.Precio,
            Activo = form.Activo,
            FotoUrl = fotoUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.tblproductos.Add(entity);
        await _db.SaveChangesAsync();

        return new ProductoDto
        {
            Id = entity.Id,
            CategoriaId = entity.CategoriaId,
            Nombre = entity.Nombre,
            Descripcion = entity.Descripcion,
            FotoUrl = entity.FotoUrl,
            Precio = entity.Precio,
            Activo = entity.Activo
        };
    }

    public async Task<ProductoDto> UpdateAsync(long id, UpdateProductoForm form)
    {
        var nombre = (form.Nombre ?? "").Trim();
        if (string.IsNullOrWhiteSpace(nombre)) throw new InvalidOperationException("Nombre requerido.");
        if (form.Precio < 0) throw new InvalidOperationException("Precio inválido.");

        var entity = await _db.tblproductos.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) throw new InvalidOperationException("Producto no encontrado.");

        var categoriaExists = await _db.tblcategorias.AnyAsync(c => c.Id == form.CategoriaId);
        if (!categoriaExists) throw new InvalidOperationException("Categoría no existe.");

        if (form.Foto != null)
        {
            // opcional: borrar foto anterior si era local
            entity.FotoUrl = await SaveProductoFotoAsync(form.Foto);
        }

        entity.CategoriaId = form.CategoriaId;
        entity.Nombre = nombre;
        entity.Descripcion = form.Descripcion;
        entity.Precio = form.Precio;
        entity.Activo = form.Activo;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new ProductoDto
        {
            Id = entity.Id,
            CategoriaId = entity.CategoriaId,
            Nombre = entity.Nombre,
            Descripcion = entity.Descripcion,
            FotoUrl = entity.FotoUrl,
            Precio = entity.Precio,
            Activo = entity.Activo
        };
    }
    public async Task<ProductoDto> SetActivoAsync(long id, bool activo)
    {
        var entity = await _db.tblproductos.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) throw new InvalidOperationException("Producto no encontrado.");

        entity.Activo = activo;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new ProductoDto
        {
            Id = entity.Id,
            CategoriaId = entity.CategoriaId,
            Nombre = entity.Nombre,
            Descripcion = entity.Descripcion,
            FotoUrl = entity.FotoUrl,
            Precio = entity.Precio,
            Activo = entity.Activo
        };
    }

    private async Task<string> SaveProductoFotoAsync(IFormFile foto)
    {
        if (foto.Length <= 0) throw new InvalidOperationException("Archivo vacío.");

        var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var folder = Path.Combine(webRoot, "uploads", "productos");
        Directory.CreateDirectory(folder);

        var ext = Path.GetExtension(foto.FileName);
        if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(folder, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await foto.CopyToAsync(stream);
        }

        // URL pública (porque está dentro de wwwroot)
        return $"/uploads/productos/{fileName}";
    }
}
