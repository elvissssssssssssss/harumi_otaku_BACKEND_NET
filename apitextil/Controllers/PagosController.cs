using Apitextil.DTOs.Pagos;
using Apitextil.Services.Pagos;
using Microsoft.AspNetCore.Mvc;

namespace Apitextil.Controllers;

[ApiController]
[Route("api/pagos")]
public class PagosController : ControllerBase
{
    private readonly IPagoService _service;
    public PagosController(IPagoService service) => _service = service;

    [HttpPost("{pagoId:long}/voucher")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SubirVoucher([FromRoute] long pagoId, [FromQuery] long usuarioId, [FromForm] UploadVoucherForm form)
        => Ok(await _service.SubirVoucherAsync(usuarioId, pagoId, form));
}
