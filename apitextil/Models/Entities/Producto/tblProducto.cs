namespace Apitextil.Models.Entities;

public class tblProducto
{
    public long Id { get; set; }
    public long CategoriaId { get; set; }

    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; }
    public string? FotoUrl { get; set; }
    public decimal Precio { get; set; }
    public bool Activo { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public tblCategoria Categoria { get; set; } = default!;
}
