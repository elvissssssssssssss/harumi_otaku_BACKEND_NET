using Apitextil.Data;
using Apitextil.DTOs.Pagos;
using Apitextil.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Apitextil.Services.Pagos;

public class PagoService : IPagoService
{
    private readonly EcommerceContext _db;
    private readonly IWebHostEnvironment _env;

    public PagoService(EcommerceContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<PagoDto> IniciarYapeAsync(long usuarioId, long ordenId, IniciarPagoYapeDto dto)
    {
        // verificar que la orden sea del usuario
        var orden = await _db.tblordenes.FirstOrDefaultAsync(o => o.Id == ordenId && o.UsuarioId == usuarioId);
        if (orden == null) throw new InvalidOperationException("Orden no encontrada.");

        // evitar 2 pagos abiertos por orden (opcional)
        var existing = await _db.tblpagos.FirstOrDefaultAsync(p => p.OrdenId == ordenId);
        if (existing != null)
        {
            var existingY = await _db.tblpago_yape_voucher.FirstOrDefaultAsync(x => x.PagoId == existing.Id);
            return new PagoDto
            {
                Id = existing.Id,
                OrdenId = existing.OrdenId,
                Metodo = existing.Metodo,
                Monto = existing.Monto,
                Estado = existing.Estado,
                YapeQrPayload = existingY?.YapeQrPayload
            };
        }

        // crear pago
        var pago = new tblPago
        {
            OrdenId = ordenId,
            Metodo = "YAPE_VOUCHER",
            Monto = orden.TotalAmount,
            Estado = "PENDIENTE",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.tblpagos.Add(pago);
        await _db.SaveChangesAsync();

        // crear row de yape
        var yape = new tblPagoYapeVoucher
        {
            PagoId = pago.Id,
            YapeQrPayload = dto.YapeQrPayload ?? "YAPE_QR_PAYLOAD_DEMO",
            VoucherImagenUrl = null,
            NroOperacion = null,
            PaidAt = null
        };

        _db.tblpago_yape_voucher.Add(yape);
        await _db.SaveChangesAsync();

        return new PagoDto
        {
            Id = pago.Id,
            OrdenId = pago.OrdenId,
            Metodo = pago.Metodo,
            Monto = pago.Monto,
            Estado = pago.Estado,
            YapeQrPayload = yape.YapeQrPayload
        };
    }

    public async Task<PagoDto> SubirVoucherAsync(long usuarioId, long pagoId, UploadVoucherForm form)
    {
        if (form.Imagen == null || form.Imagen.Length <= 0)
            throw new InvalidOperationException("Imagen requerida.");

        var pago = await _db.tblpagos.FirstOrDefaultAsync(p => p.Id == pagoId);
        if (pago == null) throw new InvalidOperationException("Pago no encontrado.");

        var orden = await _db.tblordenes.FirstOrDefaultAsync(o => o.Id == pago.OrdenId && o.UsuarioId == usuarioId);
        if (orden == null) throw new InvalidOperationException("No autorizado.");

        var yape = await _db.tblpago_yape_voucher.FirstOrDefaultAsync(x => x.PagoId == pagoId);
        if (yape == null) throw new InvalidOperationException("Pago Yape no inicializado.");

        var url = await SaveVoucherFileAsync(form.Imagen);

        yape.VoucherImagenUrl = url;
        yape.NroOperacion = form.NroOperacion;
        yape.PaidAt = DateTime.UtcNow;

        // estado pasa a EN_REVISION
        pago.Estado = "EN_REVISION";
        pago.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new PagoDto
        {
            Id = pago.Id,
            OrdenId = pago.OrdenId,
            Metodo = pago.Metodo,
            Monto = pago.Monto,
            Estado = pago.Estado,
            YapeQrPayload = yape.YapeQrPayload
        };
    }

    public async Task<PagoDto> ValidarAsync(long adminUsuarioId, long pagoId, ValidarPagoDto dto)
    {
        var pago = await _db.tblpagos.FirstOrDefaultAsync(p => p.Id == pagoId);
        if (pago == null) throw new InvalidOperationException("Pago no encontrado.");

        var resultado = (dto.Resultado ?? "").Trim().ToUpperInvariant();
        if (resultado != "CONFIRMADO" && resultado != "RECHAZADO")
            throw new InvalidOperationException("Resultado inválido.");

        _db.tblpago_validacion.Add(new tblPagoValidacion
        {
            PagoId = pagoId,
            AdminUsuarioId = adminUsuarioId,
            Resultado = resultado,
            Nota = dto.Nota,
            ValidatedAt = DateTime.UtcNow
        });

        pago.Estado = resultado == "CONFIRMADO" ? "CONFIRMADO" : "RECHAZADO";
        pago.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        var yape = await _db.tblpago_yape_voucher.FirstOrDefaultAsync(x => x.PagoId == pagoId);

        return new PagoDto
        {
            Id = pago.Id,
            OrdenId = pago.OrdenId,
            Metodo = pago.Metodo,
            Monto = pago.Monto,
            Estado = pago.Estado,
            YapeQrPayload = yape?.YapeQrPayload
        };
    }

    private async Task<string> SaveVoucherFileAsync(IFormFile file)
    {
        var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var folder = Path.Combine(webRoot, "uploads", "vouchers");
        Directory.CreateDirectory(folder);

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(folder, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/vouchers/{fileName}";
    }
}
