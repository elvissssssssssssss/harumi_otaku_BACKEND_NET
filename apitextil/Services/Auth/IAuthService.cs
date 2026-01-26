using Apitextil.DTOs.Auth;

namespace Apitextil.Services.Auth;

public interface IAuthService
{
    Task<long> RegisterAsync(AuthRegisterDto dto);
    Task<long?> LoginAsync(AuthLoginDto dto); // por ahora retorna userId (luego JWT)
}
