using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MicroserviceB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountController : ControllerBase
    {
        [HttpGet("count")]
        public IActionResult Get()
        {
           
            Log.Information("Count endpoint appelé");

            var message = "Count from MicroserviceB!";

            Log.Information("Réponse générée : {Message}", message);

            return Ok(new { Message = message });
        }
    }
}
