    namespace Apitextil.Models.Entities;

public class tblOrden
{
    public long Id { get; set; }
    public long UsuarioId { get; set; }

    public DateTime PickupAt { get; set; } // fecha/hora de recojo
    public decimal TotalAmount { get; set; }

    public long EstadoActualId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public tblUsuario Usuario { get; set; } = default!;
    public tblEstadoOrden EstadoActual { get; set; } = default!;

    public ICollection<tblOrdenItem> Items { get; set; } = new List<tblOrdenItem>();
    public ICollection<tblOrdenEstadoHistorial> HistorialEstados { get; set; } = new List<tblOrdenEstadoHistorial>();

    public tblPago? Pago { get; set; } // 1 a 1 (si mantienes 1 pago por orden)
}
