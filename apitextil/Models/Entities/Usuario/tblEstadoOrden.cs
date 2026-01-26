



public class tblEstadoOrden
{
    public long Id { get; set; }
    public string Codigo { get; set; } = default!; // CREADA, PAGO_CONFIRMADO, LISTO_PARA_RECOJO, etc.
    public string Nombre { get; set; } = default!;
}