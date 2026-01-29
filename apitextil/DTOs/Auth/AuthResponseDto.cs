namespace Apitextil.DTOs.Auth;

public class AuthResponseDto
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
