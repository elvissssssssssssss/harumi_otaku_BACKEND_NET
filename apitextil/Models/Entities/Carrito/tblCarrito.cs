namespace Apitextil.Models.Entities;

public class tblCarrito
{
    public long Id { get; set; }
    public long UsuarioId { get; set; }
    public string Estado { get; set; } = "ABIERTO"; // ABIERTO/CONVERTIDO/CANCELADO

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public tblUsuario Usuario { get; set; } = default!;
    public ICollection<tblCarritoItem> Items { get; set; } = new List<tblCarritoItem>();
}
