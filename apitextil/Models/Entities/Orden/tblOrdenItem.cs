namespace Apitextil.Models.Entities;

public class tblOrdenItem
{
    public long Id { get; set; }
    public long OrdenId { get; set; }
    public long ProductoId { get; set; }

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }

    public DateTime CreatedAt { get; set; }

    public tblOrden Orden { get; set; } = default!;
    public tblProducto Producto { get; set; } = default!;
}
