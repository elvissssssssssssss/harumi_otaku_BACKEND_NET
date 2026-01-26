using Apitextil.Data;
using Apitextil.DTOs.Carrito;
using Apitextil.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Apitextil.Services.Carrito;

public class CarritoService : ICarritoService
{
    private readonly EcommerceContext _db;

    public CarritoService(EcommerceContext db) => _db = db;

    public async Task<CarritoActualDto> GetCarritoActualAsync(long usuarioId)
    {
        var carrito = await GetOrCreateCarritoAbiertoAsync(usuarioId);
        return await BuildDtoAsync(carrito.Id, usuarioId);
    }

    public async Task<CarritoActualDto> AddItemAsync(long usuarioId, AddCarritoItemDto dto)
    {
        if (dto.Cantidad <= 0) throw new InvalidOperationException("Cantidad inválida.");

        var producto = await _db.tblproductos.FirstOrDefaultAsync(p => p.Id == dto.ProductoId && p.Activo);
        if (producto == null) throw new InvalidOperationException("Producto no existe o no está activo.");

        var carrito = await GetOrCreateCarritoAbiertoAsync(usuarioId);

        var item = await _db.tblcarrito_items
            .FirstOrDefaultAsync(i => i.CarritoId == carrito.Id && i.ProductoId == dto.ProductoId);

        if (item == null)
        {
            item = new tblCarritoItem
            {
                CarritoId = carrito.Id,
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                PrecioUnitario = producto.Precio,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.tblcarrito_items.Add(item);
        }
        else
        {
            item.Cantidad += dto.Cantidad;
            item.PrecioUnitario = producto.Precio; // opcional: mantener precio actual
            item.UpdatedAt = DateTime.UtcNow;
        }

        carrito.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return await BuildDtoAsync(carrito.Id, usuarioId);
    }

    public async Task<CarritoActualDto> UpdateItemAsync(long usuarioId, long itemId, UpdateCarritoItemDto dto)
    {
        if (dto.Cantidad <= 0) throw new InvalidOperationException("Cantidad inválida.");

        var carrito = await GetOrCreateCarritoAbiertoAsync(usuarioId);

        var item = await _db.tblcarrito_items.FirstOrDefaultAsync(i => i.Id == itemId && i.CarritoId == carrito.Id);
        if (item == null) throw new InvalidOperationException("Item no encontrado en tu carrito.");

        item.Cantidad = dto.Cantidad;
        item.UpdatedAt = DateTime.UtcNow;
        carrito.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return await BuildDtoAsync(carrito.Id, usuarioId);
    }

    public async Task<CarritoActualDto> DeleteItemAsync(long usuarioId, long itemId)
    {
        var carrito = await GetOrCreateCarritoAbiertoAsync(usuarioId);

        var item = await _db.tblcarrito_items.FirstOrDefaultAsync(i => i.Id == itemId && i.CarritoId == carrito.Id);
        if (item == null) throw new InvalidOperationException("Item no encontrado en tu carrito.");

        _db.tblcarrito_items.Remove(item);
        carrito.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return await BuildDtoAsync(carrito.Id, usuarioId);
    }

    private async Task<tblCarrito> GetOrCreateCarritoAbiertoAsync(long usuarioId)
    {
        var carrito = await _db.tblcarritos
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.Estado == "ABIERTO");

        if (carrito != null) return carrito;

        carrito = new tblCarrito
        {
            UsuarioId = usuarioId,
            Estado = "ABIERTO",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.tblcarritos.Add(carrito);
        await _db.SaveChangesAsync();
        return carrito;
    }

    private async Task<CarritoActualDto> BuildDtoAsync(long carritoId, long usuarioId)
    {
        var items = await _db.tblcarrito_items
            .Where(i => i.CarritoId == carritoId)
            .Join(_db.tblproductos,
                i => i.ProductoId,
                p => p.Id,
                (i, p) => new CarritoItemDto
                {
                    ItemId = i.Id,
                    ProductoId = p.Id,
                    Nombre = p.Nombre,
                    PrecioUnitario = i.PrecioUnitario,
                    Cantidad = i.Cantidad,
                    Subtotal = i.PrecioUnitario * i.Cantidad
                })
            .ToListAsync();

        return new CarritoActualDto
        {
            CarritoId = carritoId,
            UsuarioId = usuarioId,
            Estado = "ABIERTO",
            Items = items,
            Total = items.Sum(x => x.Subtotal)
        };
    }
}
