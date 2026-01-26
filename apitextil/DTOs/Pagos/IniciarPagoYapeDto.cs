namespace Apitextil.DTOs.Pagos;

public class IniciarPagoYapeDto
{
    public string? YapeQrPayload { get; set; } // si ya tienes el payload fijo/plantilla
    public string? Nota { get; set; }
}
