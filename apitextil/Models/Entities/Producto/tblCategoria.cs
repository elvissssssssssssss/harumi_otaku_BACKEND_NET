namespace Apitextil.Models.Entities;

public class tblCategoria
{
    public long Id { get; set; }
    public string Nombre { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<tblProducto> Productos { get; set; } = new List<tblProducto>();
}
