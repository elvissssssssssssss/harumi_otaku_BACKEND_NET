using Apitextil.Data;
using Apitextil.DTOs.Auth;
using Apitextil.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Apitextil.Services.Auth;

public class AuthService : IAuthService
{
    private readonly EcommerceContext _db;
    private readonly IConfiguration _configuration;

    public AuthService(EcommerceContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<long> RegisterAsync(AuthRegisterDto dto)
    {
        var exists = await _db.tblusuarios.AnyAsync(x => x.Email == dto.Email);
        if (exists) throw new InvalidOperationException("Email ya registrado.");

        var user = new tblUsuario
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, 12),
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

    public async Task<AuthResponseDto?> LoginAsync(AuthLoginDto dto)
    {
        var user = await _db.tblusuarios.FirstOrDefaultAsync(x => x.Email == dto.Email);
        if (user == null) return null;

        var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!ok) return null;

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Nombre = user.Nombre,
            Rol = user.Rol,
            Token = token
        };
    }

    // ✅ ACTUALIZADO: Usa JwtSettings de tu appsettings.json
    private string GenerateJwtToken(tblUsuario user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        // ✅ Lee desde JwtSettings (como lo tienes en appsettings.json)
        var secretKey = _configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JwtSettings:SecretKey no está configurado");

        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];
        var expiryHours = int.Parse(_configuration["JwtSettings:ExpiryInHours"] ?? "24");

        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim(ClaimTypes.Role, user.Rol)
            }),
            Expires = DateTime.UtcNow.AddHours(expiryHours), // ✅ Usa tu ExpiryInHours
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
