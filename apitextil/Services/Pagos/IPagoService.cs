using Apitextil.DTOs.Pagos;

namespace Apitextil.Services.Pagos;

public interface IPagoService
{
    Task<PagoDto> IniciarYapeAsync(long usuarioId, long ordenId, IniciarPagoYapeDto dto);
    Task<PagoDto> SubirVoucherAsync(long usuarioId, long pagoId, UploadVoucherForm form);
    Task<PagoDto> ValidarAsync(long adminUsuarioId, long pagoId, ValidarPagoDto dto);
}
