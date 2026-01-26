using Apitextil.Data;
using Apitextil.DTOs.Ordenes;
using Apitextil.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Apitextil.Services.Ordenes;

public class AdminOrdenService : IAdminOrdenService
{
    private readonly EcommerceContext _db;
    public AdminOrdenService(EcommerceContext db) => _db = db;

    public async Task CambiarEstadoAsync(long adminUsuarioId, long ordenId, AdminCambiarEstadoOrdenDto dto)
    {
        var orden = await _db.tblordenes.FirstOrDefaultAsync(o => o.Id == ordenId);
        if (orden == null) throw new InvalidOperationException("Orden no encontrada.");

        var nuevoCodigo = (dto.EstadoCodigo ?? "").Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(nuevoCodigo)) throw new InvalidOperationException("EstadoCodigo requerido.");

        // cargar estado actual y nuevo
        var estados = await _db.tblestados_orden.ToListAsync();
        var actual = estados.FirstOrDefault(e => e.Id == orden.EstadoActualId);
        var nuevo = estados.FirstOrDefault(e => e.Codigo == nuevoCodigo);

        if (actual == null) throw new InvalidOperationException("Estado actual inválido en la orden.");
        if (nuevo == null) throw new InvalidOperationException("Estado destino no existe.");

        if (actual.Id == nuevo.Id)
            throw new InvalidOperationException("La orden ya está en ese estado.");

        // validar transición
        if (!EsTransicionValida(actual.Codigo, nuevo.Codigo))
            throw new InvalidOperationException($"Transición no permitida: {actual.Codigo} -> {nuevo.Codigo}");

        await using var tx = await _db.Database.BeginTransactionAsync(); // atomicidad [web:328]

        orden.EstadoActualId = nuevo.Id;
        orden.UpdatedAt = DateTime.UtcNow;

        _db.tblorden_estado_historial.Add(new tblOrdenEstadoHistorial
        {
            OrdenId = orden.Id,
            EstadoId = nuevo.Id,
            CambiadoPorUsuarioId = adminUsuarioId,
            Comentario = dto.Comentario,
            FechaCambio = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    private static bool EsTransicionValida(string actual, string nuevo)
    {
        actual = (actual ?? "").Trim().ToUpperInvariant();
        nuevo = (nuevo ?? "").Trim().ToUpperInvariant();

        // terminales
        if (actual is "ENTREGADO" or "CANCELADO") return false;

        return (actual, nuevo) switch
        {
            ("CREADA", "PENDIENTE") => true,

            ("PENDIENTE", "PREPARANDO") => true,
            ("PENDIENTE", "CANCELADO") => true,

            ("PREPARANDO", "LISTO") => true,
            ("PREPARANDO", "CANCELADO") => true,

            ("LISTO", "ENTREGADO") => true,

            _ => false
        };
    }
}
