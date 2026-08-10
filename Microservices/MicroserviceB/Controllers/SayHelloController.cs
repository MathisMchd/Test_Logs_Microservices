using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MicroserviceB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SayHelloController : ControllerBase
    {
        [HttpGet("say-hello")]
        public IActionResult Get()
        {
            // Le CorrelationId, TraceId et SpanId sont ajoutés automatiquement par les enrichers Serilog
            Log.Information("SayHello endpoint appelé");

            var message = "Hello from MicroserviceB!";

            Log.Information("Réponse générée : {Message}", message);

            return Ok(new { Message = message });
        }
    }
}
