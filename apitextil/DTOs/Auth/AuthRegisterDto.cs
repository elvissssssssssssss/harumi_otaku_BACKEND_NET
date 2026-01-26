namespace Apitextil.DTOs.Auth;

public record AuthRegisterDto(string Email, string Password, string Nombre, string Apellido);
public record AuthLoginDto(string Email, string Password);
