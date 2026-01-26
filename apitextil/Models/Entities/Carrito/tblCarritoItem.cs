namespace Apitextil.Models.Entities;

public class tblCarritoItem
{
    public long Id { get; set; }
    public long CarritoId { get; set; }
    public long ProductoId { get; set; }

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public tblCarrito Carrito { get; set; } = default!;
    public tblProducto Producto { get; set; } = default!;
}
