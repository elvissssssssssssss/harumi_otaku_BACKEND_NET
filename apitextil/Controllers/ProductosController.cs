using Apitextil.DTOs.Productos;
using Apitextil.Services.Productos;
using Microsoft.AspNetCore.Mvc;
using Apitextil.DTOs.Productos;
namespace Apitextil.Controllers;

[ApiController]
[Route("api/productos")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _service;

    public ProductosController(IProductoService service) => _service = service;

    // GET /api/productos  (por defecto: activo=true)
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] long? categoriaId,
        [FromQuery] bool? activo = true)
        => Ok(await _service.GetAllAsync(categoriaId, activo));

    // GET /api/productos/{id}
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById([FromRoute] long id)
    {
        var p = await _service.GetByIdAsync(id);
        return p == null ? NotFound() : Ok(p);
    }

    // POST /api/productos  (multipart/form-data)
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CreateProductoForm form)
    {
        var created = await _service.CreateAsync(form);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    [HttpPatch("{id:long}/activo")]
    // [Authorize(Roles="ADMIN")]
    public async Task<IActionResult> SetActivo([FromRoute] long id, [FromBody] SetProductoActivoDto dto)
    => Ok(await _service.SetActivoAsync(id, dto.Activo));


    // PUT /api/productos/{id}  (multipart/form-data)
    [HttpPut("{id:long}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update([FromRoute] long id, [FromForm] UpdateProductoForm form)
        => Ok(await _service.UpdateAsync(id, form));
}
