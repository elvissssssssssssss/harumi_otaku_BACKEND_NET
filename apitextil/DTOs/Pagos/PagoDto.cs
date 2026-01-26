namespace Apitextil.DTOs.Pagos;

public class PagoDto
{
    public long Id { get; set; }
    public long OrdenId { get; set; }
    public string Metodo { get; set; } = "YAPE_VOUCHER";
    public decimal Monto { get; set; }
    public string Estado { get; set; } = "PENDIENTE";
    public string? YapeQrPayload { get; set; }
}
