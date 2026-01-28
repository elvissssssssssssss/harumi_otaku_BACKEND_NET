// Services/Ordenes/OrderNotificationService.cs
using Microsoft.Extensions.Configuration;
using PusherServer;

namespace Apitextil.Services.Ordenes
{
    public interface IOrderNotificationService
    {
        Task NotificarCambioEstadoOrden(
            long userId,
            long ordenId,
            string mensaje
        );
    }

    public class OrderNotificationService : IOrderNotificationService
    {
        private readonly Pusher _pusher;

        public OrderNotificationService(IConfiguration configuration)
        {
            var options = new PusherOptions
            {
                Cluster = configuration["Pusher:Cluster"] ?? "us2",
                Encrypted = true
            };

            _pusher = new Pusher(
                configuration["Pusher:AppId"] ?? "2107833",
                configuration["Pusher:Key"] ?? "f4e6b23dca02881c8df9",
                configuration["Pusher:Secret"] ?? "e00032aae46151b0fee5",
                options
            );
        }

        public async Task NotificarCambioEstadoOrden(
            long userId,
            long ordenId,
            string mensaje
        )
        {
            var data = new
            {
                ordenId = ordenId,
                mensaje = mensaje,
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                tipo = "orden-estado-cambiado"
            };

            await _pusher.TriggerAsync(
                $"private-user-{userId}",
                "orden-estado-cambiado",
                data
            );
        }
    }
}
