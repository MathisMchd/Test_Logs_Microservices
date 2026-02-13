
using System.Diagnostics;

public class TracePropagationHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (Activity.Current != null)
        {
            // Ajoute automatiquement traceparent pour toutes les requêtes sortantes
            request.Headers.Add("traceparent", Activity.Current.Id);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
