using Apitextil.DTOs.Ordenes;
using Apitextil.Services.Ordenes;
using Microsoft.AspNetCore.Mvc;

namespace Apitextil.Controllers;

[ApiController]
[Route("api/admin/ordenes")]
public class AdminOrdenesController : ControllerBase
{
    private readonly IAdminOrdenService _service;
    public AdminOrdenesController(IAdminOrdenService service) => _service = service;

    [HttpPatch("{id:long}/estado")]
    public async Task<IActionResult> CambiarEstado(
        [FromRoute] long id,
        [FromQuery] long adminUsuarioId,
        [FromBody] AdminCambiarEstadoOrdenDto dto)
    {
        await _service.CambiarEstadoAsync(adminUsuarioId, id, dto);
        return NoContent();
    }
}
