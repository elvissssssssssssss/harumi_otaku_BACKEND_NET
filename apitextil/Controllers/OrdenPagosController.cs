using Apitextil.DTOs.Pagos;
using Apitextil.Services.Pagos;
using Microsoft.AspNetCore.Mvc;

namespace Apitextil.Controllers;

[ApiController]
[Route("api/ordenes/{ordenId:long}/pago/yape")]
public class OrdenPagosController : ControllerBase
{
    private readonly IPagoService _service;
    public OrdenPagosController(IPagoService service) => _service = service;

    [HttpPost("iniciar")]
    public async Task<IActionResult> Iniciar([FromRoute] long ordenId, [FromQuery] long usuarioId, [FromBody] IniciarPagoYapeDto dto)
        => Ok(await _service.IniciarYapeAsync(usuarioId, ordenId, dto));
}
