using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dapr.Client;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using static Dapr.Client.Autogen.Grpc.v1.Dapr;
using DaprClient = Dapr.Client.DaprClient;

namespace EventPublisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        public EventController(ILogger<EventController> logger = null)
        {
            _logger = logger ?? NullLogger<EventController>.Instance;
        }


        [HttpPost]
        public async Task<IActionResult> PublishEvent()
        {
            var httpClient = DaprClient.CreateInvokeHttpClient("myapps1");

            //mensaje
            var requestBody = "{\"data\":{\"message\":\"Hola Mundo desde Sender\"}}";

            // Configura la solicitud POST
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(httpClient.BaseAddress+ "subscribe"),
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };
            var value = request.RequestUri;
            _logger.LogInformation("BaseAdress: {value}", value);
            // Envía la solicitud
            var response = await httpClient.SendAsync(request);

            // Verifica si la solicitud fue exitosa
            if (response.IsSuccessStatusCode)
            {
                return Ok("Evento publicado con éxito");
            }
            else
            {
                return BadRequest("Error al publicar evento");
            }
        }
    }
}
