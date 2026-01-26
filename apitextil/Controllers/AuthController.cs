using Apitextil.DTOs.Auth;
using Apitextil.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Apitextil.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service) => _service = service;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRegisterDto dto)
    {
        var id = await _service.RegisterAsync(dto);
        return CreatedAtAction(nameof(Me), new { id }, new { id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthLoginDto dto)
    {
        var userId = await _service.LoginAsync(dto);
        if (userId == null) return Unauthorized(new { message = "Credenciales inválidas" });

        // Luego aquí devuelves JWT. Por ahora devolvemos el userId.
        return Ok(new { userId });
    }

    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new { message = "Pendiente: JWT + claims" });
    }
}
