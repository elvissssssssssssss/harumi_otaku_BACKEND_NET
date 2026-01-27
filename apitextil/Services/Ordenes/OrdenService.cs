using Apitextil.Data;
using Apitextil.DTOs.Ordenes;
using Apitextil.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Apitextil.Services.Ordenes;

public class OrdenService : IOrdenService
{
    private readonly EcommerceContext _db;

    public OrdenService(EcommerceContext db) => _db = db;

    public async Task<long> CreateFromCarritoAsync(long usuarioId, CreateOrdenDto dto)
    {
        // 1) carrito ABIERTO
        var carrito = await _db.tblcarritos
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.Estado == "ABIERTO");

        if (carrito == null) throw new InvalidOperationException("No tienes carrito ABIERTO.");

        var items = await _db.tblcarrito_items
            .Where(i => i.CarritoId == carrito.Id)
            .ToListAsync();

        if (items.Count == 0) throw new InvalidOperationException("Tu carrito está vacío.");

        // 2) estado inicial de orden (de tu tabla tblestados_orden)
        var estadoInit = await _db.tblestados_orden
            .OrderBy(e => e.Id)
            .FirstOrDefaultAsync();

        if (estadoInit == null) throw new InvalidOperationException("No hay estados de orden configurados.");

        // 3) transacción (orden + items + historial + carrito)
        await using var tx = await _db.Database.BeginTransactionAsync(); // atomicidad [web:328]

        var total = items.Sum(x => x.PrecioUnitario * x.Cantidad);

        var orden = new tblOrden
        {
            UsuarioId = usuarioId,
            PickupAt = dto.PickupAt,
            TotalAmount = total,
            EstadoActualId = estadoInit.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.tblordenes.Add(orden);
        await _db.SaveChangesAsync(); // para obtener orden.Id

        foreach (var ci in items)
        {
            _db.tblorden_items.Add(new tblOrdenItem
            {
                OrdenId = orden.Id,
                ProductoId = ci.ProductoId,
                Cantidad = ci.Cantidad,
                PrecioUnitario = ci.PrecioUnitario,
                Subtotal = ci.PrecioUnitario * ci.Cantidad,
                CreatedAt = DateTime.UtcNow
            });
        }

        _db.tblorden_estado_historial.Add(new tblOrdenEstadoHistorial
        {
            OrdenId = orden.Id,
            EstadoId = estadoInit.Id,
            CambiadoPorUsuarioId = usuarioId,
            Comentario = "Orden creada",
            FechaCambio = DateTime.UtcNow
        });

        carrito.Estado = "CONVERTIDO";
        carrito.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        return orden.Id;
    }

    public async Task<List<OrdenDto>> GetAllByUsuarioAsync(long usuarioId)
    {
        var ordenes = await _db.tblordenes
            .Where(o => o.UsuarioId == usuarioId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrdenDto
            {
                Id = o.Id,
                UsuarioId = o.UsuarioId,
                PickupAt = o.PickupAt,
                TotalAmount = o.TotalAmount,
                EstadoActualId = o.EstadoActualId,
                CreatedAt = o.CreatedAt
            })
            .ToListAsync();

        return ordenes;
    }

    public async Task<OrdenDto?> GetByIdAsync(long usuarioId, long ordenId)
    {
        var orden = await _db.tblordenes
            .Where(o => o.Id == ordenId && o.UsuarioId == usuarioId)
            .Select(o => new OrdenDto
            {
                Id = o.Id,
                UsuarioId = o.UsuarioId,
                PickupAt = o.PickupAt,
                TotalAmount = o.TotalAmount,
                EstadoActualId = o.EstadoActualId,
                CreatedAt = o.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (orden == null) return null;

        var estado = await _db.tblestados_orden.FirstOrDefaultAsync(e => e.Id == orden.EstadoActualId);
        if (estado != null)
        {
            orden.EstadoCodigo = estado.Codigo;
            orden.EstadoNombre = estado.Nombre;
        }

        orden.Items = await _db.tblorden_items
            .Where(i => i.OrdenId == ordenId)
            .Join(_db.tblproductos,
                i => i.ProductoId,
                p => p.Id,
                (i, p) => new OrdenItemDto
                {
                    Id = i.Id,
                    ProductoId = p.Id,
                    Nombre = p.Nombre,
                    Cantidad = i.Cantidad,
                    PrecioUnitario = i.PrecioUnitario,
                    Subtotal = i.Subtotal
                })
            .ToListAsync();

        return orden;
    }
    public async Task<List<OrdenDto>> GetAllAsync()
    {
        return await (
            from o in _db.tblordenes
            join e in _db.tblestados_orden on o.EstadoActualId equals e.Id
            join oi in _db.tblorden_items on o.Id equals oi.OrdenId into items
            orderby o.CreatedAt descending
            select new OrdenDto
            {
                Id = o.Id,
                UsuarioId = o.UsuarioId,
                PickupAt = o.PickupAt,
                TotalAmount = o.TotalAmount,
                EstadoActualId = o.EstadoActualId,
                EstadoCodigo = e.Codigo,
                EstadoNombre = e.Nombre,
                CreatedAt = o.CreatedAt,

                ItemsCount = items.Count(),
                ItemsCantidadTotal = items.Sum(x => (int?)x.Cantidad) ?? 0,
                ItemsSubtotalTotal = items.Sum(x => (decimal?)x.Subtotal) ?? 0m
            }
        ).ToListAsync();
    }


    public async Task<List<OrdenEstadoHistorialDto>> GetHistorialAsync(long usuarioId, long ordenId)
    {
        var owner = await _db.tblordenes.AnyAsync(o => o.Id == ordenId && o.UsuarioId == usuarioId);
        if (!owner) throw new InvalidOperationException("Orden no encontrada.");

        var list = await _db.tblorden_estado_historial
            .Where(h => h.OrdenId == ordenId)
            .OrderBy(h => h.FechaCambio)
            .Select(h => new OrdenEstadoHistorialDto
            {
                Id = h.Id,
                OrdenId = h.OrdenId,
                EstadoId = h.EstadoId,
                CambiadoPorUsuarioId = h.CambiadoPorUsuarioId,
                Comentario = h.Comentario,
                FechaCambio = h.FechaCambio
            })
            .ToListAsync();

        // completar nombres de estado (simple)
        var estados = await _db.tblestados_orden.ToListAsync();
        foreach (var h in list)
        {
            var e = estados.FirstOrDefault(x => x.Id == h.EstadoId);
            h.EstadoCodigo = e?.Codigo;
            h.EstadoNombre = e?.Nombre;
        }

        return list;
    }
}
