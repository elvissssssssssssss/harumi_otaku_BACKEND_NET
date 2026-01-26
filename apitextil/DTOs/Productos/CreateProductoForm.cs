using Microsoft.AspNetCore.Http;

namespace Apitextil.DTOs.Productos;

public class CreateProductoForm
{
    public long CategoriaId { get; set; }
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public bool Activo { get; set; } = true;

    // archivo (opcional)
    public IFormFile? Foto { get; set; }
}
