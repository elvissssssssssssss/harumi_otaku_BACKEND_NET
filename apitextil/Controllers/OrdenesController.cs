using Apitextil.DTOs.Ordenes;
using Apitextil.Services.Ordenes;
using Microsoft.AspNetCore.Mvc;

namespace Apitextil.Controllers;

[ApiController]
[Route("api/ordenes")]
public class OrdenesController : ControllerBase
{
    private readonly IOrdenService _service;

    public OrdenesController(IOrdenService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromQuery] long usuarioId, [FromBody] CreateOrdenDto dto)
    {
        try
        {
            var id = await _service.CreateFromCarritoAsync(usuarioId, dto);
            return CreatedAtAction(nameof(GetById), new { id, usuarioId }, new { id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] long usuarioId)
        => Ok(await _service.GetAllByUsuarioAsync(usuarioId));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById([FromRoute] long id, [FromQuery] long usuarioId)
    {
        var orden = await _service.GetByIdAsync(usuarioId, id);
        return orden == null ? NotFound() : Ok(orden);
    }

    [HttpGet("{id:long}/historial-estados")]
    public async Task<IActionResult> Historial([FromRoute] long id, [FromQuery] long usuarioId)
        => Ok(await _service.GetHistorialAsync(usuarioId, id));
}
