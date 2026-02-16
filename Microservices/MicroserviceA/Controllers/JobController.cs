using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Diagnostics;

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
            var request = new HttpRequestMessage(HttpMethod.Get, "http://microservice-b:8080/say-hello");
            request.Headers.Add("X-Correlation-ID", correlationId);

            var response = await client.SendAsync(request);
            var message = await response.Content.ReadAsStringAsync();

            Log.Information("Réponse de ServiceB reçue : {Message}", message);

            return Ok(new { Message = "Job terminé", ServiceBResponse = message });
        }

        [HttpPost("double-job")]
        public async Task<IActionResult> DoubleJob()
        {
            using var spanA = new Activity("DoubleJob").Start(); // Span global

            Log.Information("- Lancement de deux jobs en parallèle...");

            // Lancer les deux jobs en parallèle, chaque tâche crée son propre span
            var job1 = ExecuteJobAsync("Job-1", "http://microservice-b:8080/SayHello/say-hello", spanA);
            var job2 = ExecuteJobAsync("Job-2", "http://microservice-b:8080/Count/count", spanA);

            var results = await Task.WhenAll(job1, job2);

            Log.Information("- Les deux jobs sont terminés");

            return Ok(new
            {
                Message = "Deux jobs exécutés en parallèle",
                Results = results
            });
        }

        private async Task<object> ExecuteJobAsync(string jobName, string url, Activity parentSpan)
        {
            using var span = new Activity(jobName);
            span.SetParentId(parentSpan.Id); // définit parent
            span.Start();

            Log.Information(" ==> {JobName} démarré", jobName);

            var client = _httpClientFactory.CreateClient();

            Log.Information(" - {JobName} appelle {Url}", jobName, url);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Propagation automatique de trace avec HttpClient
            request.Headers.Add("traceparent", span.Id);

            var response = await client.SendAsync(request);
            var message = await response.Content.ReadAsStringAsync();

            Log.Information(" - {JobName} réponse reçue : {Message}", jobName, message);

            return new
            {
                Job = jobName,
                ServiceBResponse = message
            };
        }

    }

}