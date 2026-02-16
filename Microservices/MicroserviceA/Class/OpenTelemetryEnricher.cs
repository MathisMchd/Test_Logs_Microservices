using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

public class OpenTelemetryEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        // Récupère l'activité courante
        var activity = Activity.Current;

        // TraceId
        var traceId = activity?.TraceId.ToString() ?? string.Empty;
        // SpanId courant
        var spanId = activity?.SpanId.ToString() ?? string.Empty;
        // Span parent
        var parentSpanId = activity?.ParentSpanId.ToString() ?? string.Empty;

        // Ajoute dans le log
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", traceId));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", spanId));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ParentSpanId", parentSpanId));
    }
}
