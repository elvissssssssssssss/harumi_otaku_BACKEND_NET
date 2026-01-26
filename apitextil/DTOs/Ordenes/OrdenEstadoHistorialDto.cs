namespace Apitextil.DTOs.Ordenes;

public class OrdenEstadoHistorialDto
{
    public long Id { get; set; }
    public long OrdenId { get; set; }
    public long EstadoId { get; set; }
    public string? EstadoCodigo { get; set; }
    public string? EstadoNombre { get; set; }
    public long? CambiadoPorUsuarioId { get; set; }
    public string? Comentario { get; set; }
    public DateTime FechaCambio { get; set; }
}
