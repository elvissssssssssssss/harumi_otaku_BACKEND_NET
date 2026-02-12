// Archivo: DTOs/Auth/UserResponseDto.cs
namespace Apitextil.DTOs.Auth; // Verifica que este nombre coincida con tu carpeta

public class UserResponseDto
{
    public long Id { get; set; }
    public string Email { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Rol { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}