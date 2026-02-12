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
    // Usaremos un solo nombre para la base de datos: _context
    private readonly EcommerceContext _context;
    private readonly IConfiguration _configuration;

    // ✅ UN SOLO CONSTRUCTOR: Recibe ambas dependencias
    public AuthService(EcommerceContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<long> RegisterAsync(AuthRegisterDto dto)
    {
        // Cambiado _db por _context
        var exists = await _context.tblusuarios.AnyAsync(x => x.Email == dto.Email);
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

        _context.tblusuarios.Add(user);
        await _context.SaveChangesAsync();
        return user.Id;
    }

    public async Task<AuthResponseDto?> LoginAsync(AuthLoginDto dto)
    {
        var user = await _context.tblusuarios.FirstOrDefaultAsync(x => x.Email == dto.Email);
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

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        // ✅ Ahora _context funcionará perfectamente aquí
        return await _context.tblusuarios
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                Email = u.Email,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Rol = u.Rol,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }

    private string GenerateJwtToken(tblUsuario user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKey = _configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("SecretKey no configurada");

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
            Expires = DateTime.UtcNow.AddHours(24),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}