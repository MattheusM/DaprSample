using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Dapr.Client;

[ApiController]
public class EventReceiverController : ControllerBase
{
    private readonly ILogger<EventReceiverController> _logger;
    private readonly DaprClient _daprClient;

    public EventReceiverController(ILogger<EventReceiverController> logger = null, DaprClient daprClient = null)
    {
        _logger = logger ?? NullLogger<EventReceiverController>.Instance;
        _daprClient = daprClient ?? new DaprClientBuilder().Build();
    }

    [Route("subscribe")]
    [HttpPost]
    public async Task<IActionResult> Subscribe()
    {
        string value = await ReadRequestBodyAsString(Request);
        _logger.LogInformation("Received event: {value}", value);
        return Ok();
    }

    private async Task<string> ReadRequestBodyAsString(HttpRequest request)
    {
        using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
        {
            return await reader.ReadToEndAsync();
        }
    }
}




