using Apitextil.Data;
using Apitextil.DTOs.Auth;
using Apitextil.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Apitextil.Services.Auth;

public class AuthService : IAuthService
{
    private readonly EcommerceContext _db;

    public AuthService(EcommerceContext db) => _db = db;

    public async Task<long> RegisterAsync(AuthRegisterDto dto)
    {
        var exists = await _db.tblusuarios.AnyAsync(x => x.Email == dto.Email);
        if (exists) throw new InvalidOperationException("Email ya registrado.");

        var user = new tblUsuario
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Rol = "CLIENTE",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.tblusuarios.Add(user);
        await _db.SaveChangesAsync();
        return user.Id;
    }

    public async Task<long?> LoginAsync(AuthLoginDto dto)
    {
        var user = await _db.tblusuarios.FirstOrDefaultAsync(x => x.Email == dto.Email);
        if (user == null) return null;

        var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        return ok ? user.Id : null;
    }
}
