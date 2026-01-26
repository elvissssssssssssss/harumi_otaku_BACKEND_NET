using Apitextil.DTOs.Categorias;
using Apitextil.Services.Categorias;
using Microsoft.AspNetCore.Mvc;

namespace Apitextil.Controllers;

[ApiController]
[Route("api/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _service;

    public CategoriasController(ICategoriaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById([FromRoute] long id)
    {
        var cat = await _service.GetByIdAsync(id);
        return cat == null ? NotFound() : Ok(cat);
    }

    [HttpPost]
    // Cuando ya tengas JWT con rol, activa esto:
    // [Authorize(Roles = "ADMIN")]  // roles en ASP.NET Core [web:273]
    public async Task<IActionResult> Create([FromBody] CreateCategoriaDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    // [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UpdateCategoriaDto dto)
        => Ok(await _service.UpdateAsync(id, dto));

    [HttpDelete("{id:long}")]
    // [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
