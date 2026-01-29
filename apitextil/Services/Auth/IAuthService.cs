using Apitextil.DTOs.Auth;

namespace Apitextil.Services.Auth;

public interface IAuthService
{
    Task<long> RegisterAsync(AuthRegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(AuthLoginDto dto); // ✅ Cambiado de long? a AuthResponseDto?
}
