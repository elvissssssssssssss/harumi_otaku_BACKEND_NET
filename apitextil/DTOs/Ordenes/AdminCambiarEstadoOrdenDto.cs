namespace Apitextil.DTOs.Ordenes;

public class AdminCambiarEstadoOrdenDto
{
    public string EstadoCodigo { get; set; } = default!; // CREADA/PENDIENTE/PREPARANDO/LISTO/ENTREGADO/CANCELADO
    public string? Comentario { get; set; }
}
