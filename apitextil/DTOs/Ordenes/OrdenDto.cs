namespace Apitextil.DTOs.Ordenes;

public class OrdenDto
{
    public long Id { get; set; }
    public long UsuarioId { get; set; }
    public DateTime PickupAt { get; set; }
    public decimal TotalAmount { get; set; }
    public long EstadoActualId { get; set; }
    public string? EstadoCodigo { get; set; }
    public string? EstadoNombre { get; set; }
    public DateTime CreatedAt { get; set; }

    // ✅ agregados por orden
    public int ItemsCount { get; set; }
    public int ItemsCantidadTotal { get; set; }
    public decimal ItemsSubtotalTotal { get; set; }
    // ... campos existentes
    public long? PagoId { get; set; }
    public string? PagoEstado { get; set; }  // "PENDIENTE", "EN_REVISION", "CONFIRMADO", "RECHAZADO"
                                             // ✅ Agrega info del voucher Yape
    public string? VoucherImagenUrl { get; set; }
    public string? NroOperacion { get; set; }
    public DateTime? PaidAt { get; set; }
    public List<OrdenItemDto> Items { get; set; } = new();
}

public class OrdenItemDto
{
    public long Id { get; set; }
    public long ProductoId { get; set; }
    public string Nombre { get; set; } = default!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
