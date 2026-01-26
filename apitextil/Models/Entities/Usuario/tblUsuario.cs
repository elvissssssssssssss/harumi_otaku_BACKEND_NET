namespace Apitextil.Models.Entities;

public class tblUsuario
{
    public long Id { get; set; }
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public string Apellido { get; set; } = default!;
    public string Rol { get; set; } = "CLIENTE"; // CLIENTE/ADMIN

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<tblCarrito> Carritos { get; set; } = new List<tblCarrito>();
    public ICollection<tblOrden> Ordenes { get; set; } = new List<tblOrden>();

    // Para validaciones de pago hechas por admin
    public ICollection<tblPagoValidacion> ValidacionesRealizadas { get; set; } = new List<tblPagoValidacion>();
}
