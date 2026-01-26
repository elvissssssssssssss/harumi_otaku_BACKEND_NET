namespace Apitextil.DTOs.Carrito;

public class CarritoActualDto
{
    public long CarritoId { get; set; }
    public long UsuarioId { get; set; }
    public string Estado { get; set; } = default!;
    public List<CarritoItemDto> Items { get; set; } = new();
    public decimal Total { get; set; }
}

public class CarritoItemDto
{
    public long ItemId { get; set; }
    public long ProductoId { get; set; }
    public string Nombre { get; set; } = default!;
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
    public decimal Subtotal { get; set; }
}
