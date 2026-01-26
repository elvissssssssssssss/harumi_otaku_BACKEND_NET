using Apitextil.Models.Entities;

public class tblPago
{
    public long Id { get; set; }
    public long OrdenId { get; set; }
    public string Metodo { get; set; } = "YAPE_VOUCHER";
    public decimal Monto { get; set; }
    public string Estado { get; set; } = "PENDIENTE";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public tblOrden Orden { get; set; } = default!;

    public tblPagoYapeVoucher? PagoYapeVoucher { get; set; }  // <- SOLO ESTA
    public tblPagoValidacion? Validacion { get; set; }
}
