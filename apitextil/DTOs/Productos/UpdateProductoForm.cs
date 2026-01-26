using Microsoft.AspNetCore.Http;

namespace Apitextil.DTOs.Productos;

public class UpdateProductoForm
{
    public long CategoriaId { get; set; }
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public bool Activo { get; set; } = true;

    // si mandas foto, reemplaza; si no mandas, conserva la anterior
    public IFormFile? Foto { get; set; }
}
