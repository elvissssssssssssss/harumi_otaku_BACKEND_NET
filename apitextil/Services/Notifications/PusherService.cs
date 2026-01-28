using PusherServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Apitextil.Services.Notifications;

public class PusherService : IPusherService
{
    private readonly Pusher _pusher;
    private readonly ILogger<PusherService> _logger;

    public PusherService(IConfiguration configuration, ILogger<PusherService> logger)
    {
        _logger = logger;

        var pusherOptions = new PusherOptions
        {
            Cluster = configuration["Pusher:Cluster"] ?? "us2",
            Encrypted = true
        };

        _pusher = new Pusher(
            configuration["Pusher:AppId"] ?? "2107833",
            configuration["Pusher:Key"] ?? "f4e6b23dca02881c8df9",
            configuration["Pusher:Secret"] ?? "e00032aae46151b0fee5",
            pusherOptions
        );

        _logger.LogInformation("✅ PusherService inicializado - Cluster: {Cluster}", pusherOptions.Cluster);
    }

    private static string GetChannel(long usuarioId) => $"private-user-{usuarioId}";

    public async Task SendOrdenCreadaAsync(long ordenId, long usuarioId, decimal total)
    {
        try
        {
            var payload = new
            {
                ordenId,
                usuarioId,
                total,
                tipo = "orden-creada",
                timestamp = DateTime.UtcNow,
                mensaje = $"Tu orden #{ordenId} ha sido creada exitosamente por S/{total:F2}"
            };

            var result = await _pusher.TriggerAsync(
                GetChannel(usuarioId),
                "orden-creada",
                payload
            );

            LogResultado(result, "orden-creada", usuarioId, ordenId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al enviar evento orden-creada para orden {OrdenId}", ordenId);
        }
    }

    public async Task SendOrdenEstadoCambiadoAsync(
        long ordenId,
        long usuarioId,
        long nuevoEstadoId,
        string nuevoEstadoCodigo,
        string nuevoEstadoNombre,
        string? comentario = null)
    {
        try
        {
            var payload = new
            {
                ordenId,
                usuarioId,
                nuevoEstadoId,
                nuevoEstadoCodigo,
                nuevoEstadoNombre,
                comentario,
                tipo = "orden-estado-cambiado",
                timestamp = DateTime.UtcNow,
                mensaje = ObtenerMensajeSegunEstado(nuevoEstadoCodigo, nuevoEstadoNombre)
            };

            var result = await _pusher.TriggerAsync(
                GetChannel(usuarioId),
                "orden-estado-cambiado",
                payload
            );

            LogResultado(result, "orden-estado-cambiado", usuarioId, ordenId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al enviar evento orden-estado-cambiado para orden {OrdenId}", ordenId);
        }
    }

    public async Task SendPagoActualizadoAsync(
        long pagoId,
        long ordenId,
        long usuarioId,
        string nuevoEstado,
        string metodoPago)
    {
        try
        {
            var payload = new
            {
                pagoId,
                ordenId,
                usuarioId,
                nuevoEstado,
                metodoPago,
                tipo = "pago-actualizado",
                timestamp = DateTime.UtcNow,
                mensaje = nuevoEstado == "CONFIRMADO"
                    ? "¡Tu pago ha sido confirmado! Tu pedido está siendo preparado 🎉"
                    : nuevoEstado == "RECHAZADO"
                        ? "Tu pago fue rechazado. Por favor, verifica tu voucher."
                        : "Tu pago está en revisión ⏳"
            };

            var result = await _pusher.TriggerAsync(
                GetChannel(usuarioId),
                "pago-actualizado",
                payload
            );

            LogResultado(result, "pago-actualizado", usuarioId, ordenId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al enviar evento pago-actualizado para pago {PagoId}", pagoId);
        }
    }

    public async Task SendOrdenListaParaRecogerAsync(
        long ordenId,
        long usuarioId,
        DateTime? pickupAt)
    {
        try
        {
            var horaRecojo = pickupAt?.ToString("hh:mm tt") ?? "Pronto";

            var payload = new
            {
                ordenId,
                usuarioId,
                horaRecojo,
                tipo = "orden-lista-recoger",
                timestamp = DateTime.UtcNow,
                mensaje = $"¡Tu pedido está listo para recoger! 🍜 Hora: {horaRecojo}"
            };

            var result = await _pusher.TriggerAsync(
                GetChannel(usuarioId),
                "orden-lista-recoger",
                payload
            );

            LogResultado(result, "orden-lista-recoger", usuarioId, ordenId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al enviar evento orden-lista-recoger para orden {OrdenId}", ordenId);
        }
    }

    public async Task SendOrdenCanceladaAsync(
        long ordenId,
        long usuarioId,
        string? motivo)
    {
        try
        {
            var payload = new
            {
                ordenId,
                usuarioId,
                motivo,
                tipo = "orden-cancelada",
                timestamp = DateTime.UtcNow,
                mensaje = $"Tu orden ha sido cancelada. {(string.IsNullOrWhiteSpace(motivo) ? "" : $"Motivo: {motivo}")}"
            };

            var result = await _pusher.TriggerAsync(
                GetChannel(usuarioId),
                "orden-cancelada",
                payload
            );

            LogResultado(result, "orden-cancelada", usuarioId, ordenId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al enviar evento orden-cancelada para orden {OrdenId}", ordenId);
        }
    }

    private void LogResultado(ITriggerResult result, string evento, long usuarioId, long ordenId)
    {
        if (result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            _logger.LogInformation("✅ Evento {Evento} enviado - Usuario: {UsuarioId}, Orden: {OrdenId}",
                evento, usuarioId, ordenId);
        }
        else
        {
            _logger.LogWarning("⚠️ Error al enviar evento {Evento}: {StatusCode} - {Body}",
                evento, result.StatusCode, result.Body);
        }
    }

    private static string ObtenerMensajeSegunEstado(string codigoEstado, string nombreEstado)
    {
        return codigoEstado.ToUpperInvariant() switch
        {
            "CREADA" => "Tu orden ha sido creada 📝",
            "PENDIENTE" => "Tu orden está pendiente de pago 💳",
            "PREPARANDO" => "¡Tu pedido está en preparación! 👨‍🍳",
            "LISTO" => "¡Tu pedido está listo para recoger! 🎉",
            "ENTREGADO" => "¡Disfruta tu comida! 🍜✨",
            "CANCELADO" => "Tu orden ha sido cancelada ❌",
            _ => $"Estado actualizado: {nombreEstado}"
        };
    }
}
