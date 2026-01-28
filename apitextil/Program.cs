using Apitextil.Services.Notifications;
using Apitextil.Data;
using Apitextil.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


using System.Text;


using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();


// JSON camelCase
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Upload limit (10MB)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// DB (MySQL + Pomelo)
builder.Services.AddDbContext<EcommerceContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(cs, ServerVersion.AutoDetect(cs));
});

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<Apitextil.Services.Carrito.ICarritoService, Apitextil.Services.Carrito.CarritoService>();
builder.Services.AddScoped<Apitextil.Services.Categorias.ICategoriaService, Apitextil.Services.Categorias.CategoriaService>();
builder.Services.AddScoped<Apitextil.Services.Productos.IProductoService, Apitextil.Services.Productos.ProductoService>();
builder.Services.AddScoped<Apitextil.Services.Ordenes.IOrdenService, Apitextil.Services.Ordenes.OrdenService>();
builder.Services.AddScoped<Apitextil.Services.Pagos.IPagoService, Apitextil.Services.Pagos.PagoService>();
builder.Services.AddScoped<Apitextil.Services.Ordenes.IAdminOrdenService, Apitextil.Services.Ordenes.AdminOrdenService>();
builder.Services.AddSingleton<IPusherService, PusherService>();



// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["JwtSettings:SecretKey"]
                    ?? "TuSupercalifragilisticoSecretoDeAlMenos32Caracteres")),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Swagger + JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Harumi", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Authorization: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Asegurar wwwroot + carpeta vouchers
var webRoot = app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
Directory.CreateDirectory(webRoot);
Directory.CreateDirectory(Path.Combine(webRoot, "uploads", "vouchers"));

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// NO UseHttpsRedirection si tu launchSettings es solo http (evita problemas). [web:193]

app.UseStaticFiles(); // sirve wwwroot (incluye /uploads/...) [web:52]

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
