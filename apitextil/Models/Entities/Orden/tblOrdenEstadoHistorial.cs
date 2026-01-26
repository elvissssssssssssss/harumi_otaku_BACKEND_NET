namespace Apitextil.Models.Entities;

public class tblOrdenEstadoHistorial
{
    public long Id { get; set; }
    public long OrdenId { get; set; }
    public long EstadoId { get; set; }

    public long? CambiadoPorUsuarioId { get; set; }
    public string? Comentario { get; set; }
    public DateTime FechaCambio { get; set; }

    public tblOrden Orden { get; set; } = default!;
    public tblEstadoOrden Estado { get; set; } = default!;
    public tblUsuario? CambiadoPorUsuario { get; set; }
}
