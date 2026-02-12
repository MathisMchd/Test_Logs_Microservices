using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MicroserviceA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public JobController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("start-job")]
        public async Task<IActionResult> StartJob()
        {
            var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                                ?? Guid.NewGuid().ToString();

            Log.Information("Job démarré");

            // Simulation de retries
            for (int retry = 1; retry <= 2; retry++)
            {
                try
                {
                    if (retry < 2) throw new Exception("Échec simulé");
                    Log.Information("Job exécuté avec succès après {RetryCount} retries", retry);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Retry {RetryCount} en cours", retry);
                }
            }

            // Appel HTTP vers ServiceB
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5002/say-hello");
            request.Headers.Add("X-Correlation-ID", correlationId);

            var response = await client.SendAsync(request);
            var message = await response.Content.ReadAsStringAsync();

            Log.Information("Réponse de ServiceB reçue : {Message}", message);

            return Ok(new { Message = "Job terminé", ServiceBResponse = message });
        }
    }

}