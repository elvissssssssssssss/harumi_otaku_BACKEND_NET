namespace Apitextil.Models.Entities;

public class tblPagoValidacion
{
    public long Id { get; set; }
    public long PagoId { get; set; }
    public long AdminUsuarioId { get; set; }

    public string Resultado { get; set; } = default!; // CONFIRMADO/RECHAZADO
    public string? Nota { get; set; }
    public DateTime ValidatedAt { get; set; }

    public tblPago Pago { get; set; } = default!;
    public tblUsuario AdminUsuario { get; set; } = default!;
}
