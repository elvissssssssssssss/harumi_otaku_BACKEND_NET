namespace Apitextil.Services.Notifications;  // ✅ A MAYÚSCULA

public interface IPusherService
{
    Task SendOrdenCreadaAsync(long ordenId, long usuarioId, decimal total);

    Task SendOrdenEstadoCambiadoAsync(
        long ordenId,
        long usuarioId,
        long nuevoEstadoId,           // ✅ CAMBIADO A long
        string nuevoEstadoCodigo,
        string nuevoEstadoNombre,
        string? comentario = null);   // ✅ AGREGADO nullable

    Task SendPagoActualizadoAsync(
        long pagoId,
        long ordenId,
        long usuarioId,
        string nuevoEstado,
        string metodoPago);

    Task SendOrdenListaParaRecogerAsync(
        long ordenId,
        long usuarioId,
        DateTime? pickupAt);

    Task SendOrdenCanceladaAsync(
        long ordenId,
        long usuarioId,
        string? motivo);              // ✅ AGREGADO nullable
}
