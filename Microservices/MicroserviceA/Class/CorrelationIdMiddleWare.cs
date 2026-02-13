namespace MicroserviceA.Class
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string HeaderKey = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 1. Récupérer le CorrelationId depuis le header ou en créer un nouveau
            var correlationId = context.Request.Headers[HeaderKey].FirstOrDefault()
                                ?? Guid.NewGuid().ToString();

            // 2. Ajouter le CorrelationId dans le LogContext
            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            {
                // 3. Optionnel : renvoyer le CorrelationId dans la réponse
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers[HeaderKey] = correlationId;
                    return Task.CompletedTask;
                });

                // 4. Continuer le pipeline
                await _next(context);
            }
        }

    }
}

