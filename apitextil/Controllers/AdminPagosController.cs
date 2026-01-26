using Apitextil.DTOs.Pagos;
using Apitextil.Services.Pagos;
using Microsoft.AspNetCore.Mvc;

namespace Apitextil.Controllers;

[ApiController]
[Route("api/admin/pagos")]
public class AdminPagosController : ControllerBase
{
    private readonly IPagoService _service;
    public AdminPagosController(IPagoService service) => _service = service;

    [HttpPost("{pagoId:long}/validar")]
    // luego: [Authorize(Roles="ADMIN")] [web:273]
    public async Task<IActionResult> Validar([FromRoute] long pagoId, [FromQuery] long adminUsuarioId, [FromBody] ValidarPagoDto dto)
        => Ok(await _service.ValidarAsync(adminUsuarioId, pagoId, dto));
}
