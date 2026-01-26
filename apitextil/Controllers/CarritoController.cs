using Apitextil.DTOs.Carrito;
using Apitextil.Services.Carrito;
using Microsoft.AspNetCore.Mvc;

namespace Apitextil.Controllers;

[ApiController]
[Route("api/carrito")]
public class CarritoController : ControllerBase
{
    private readonly ICarritoService _service;

    public CarritoController(ICarritoService service) => _service = service;

    // TEMP: pasamos usuarioId por query mientras aún no usas JWT en serio.
    // Luego lo cambiamos a User.Claims.
    [HttpGet("actual")]
    public async Task<IActionResult> GetActual([FromQuery] long usuarioId)
        => Ok(await _service.GetCarritoActualAsync(usuarioId));

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromQuery] long usuarioId, [FromBody] AddCarritoItemDto dto)
        => Ok(await _service.AddItemAsync(usuarioId, dto));

    [HttpPatch("items/{itemId:long}")]
    public async Task<IActionResult> UpdateItem([FromQuery] long usuarioId, [FromRoute] long itemId, [FromBody] UpdateCarritoItemDto dto)
        => Ok(await _service.UpdateItemAsync(usuarioId, itemId, dto));

    [HttpDelete("items/{itemId:long}")]
    public async Task<IActionResult> DeleteItem([FromQuery] long usuarioId, [FromRoute] long itemId)
        => Ok(await _service.DeleteItemAsync(usuarioId, itemId));
}
