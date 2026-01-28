using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PusherServer;

namespace apitextil.Controllers
{
    [ApiController]
    [Route("api/pusher")]
    public class PusherAuthController : ControllerBase
    {
        private readonly Pusher _pusher;

        public PusherAuthController(IConfiguration configuration)
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

        // POST: /api/pusher/auth
        [HttpPost("auth")]
        public IActionResult Auth([FromBody] PusherAuthRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.socket_id) || string.IsNullOrWhiteSpace(request.channel_name))
            {
                return BadRequest(new { message = "socket_id y channel_name son requeridos" });
            }

            // Esperamos un canal del tipo "private-user-6"
            if (!request.channel_name.StartsWith("private-user-"))
            {
                return BadRequest(new { message = "Formato de canal inválido" });
            }

            var userIdPart = request.channel_name.Replace("private-user-", "");
            if (!int.TryParse(userIdPart, out var userId) || userId <= 0)
            {
                return BadRequest(new { message = "userId inválido en channel_name" });
            }

            // Aquí podrías validar en BD si el userId existe, si quieres

            var authResponse = _pusher.Authenticate(request.channel_name, request.socket_id);

            // Devolver el JSON EXACTO que espera Pusher: { "auth": "key:signature" }
            return Content(authResponse.ToJson(), "application/json");
        }
    }

    public class PusherAuthRequest
    {
        public string socket_id { get; set; } = string.Empty;
        public string channel_name { get; set; } = string.Empty;
    }
}
