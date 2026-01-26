public class tblPagoYapeVoucher
{
    public long PagoId { get; set; }
    public string? YapeQrPayload { get; set; }
    public string? VoucherImagenUrl { get; set; }
    public string? NroOperacion { get; set; }
    public DateTime? PaidAt { get; set; }

    public tblPago Pago { get; set; } = default!;
}
