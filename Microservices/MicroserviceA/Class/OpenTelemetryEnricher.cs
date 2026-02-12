using System;

public class OpenTelemetryEnricher : Serilog.Core.ILogEventEnricher
{
    public void Enrich(Serilog.Events.LogEvent logEvent, Serilog.Core.ILogEventPropertyFactory propertyFactory)
    {
        var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? string.Empty;
        var spanId = System.Diagnostics.Activity.Current?.SpanId.ToString() ?? string.Empty;

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", traceId));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", spanId));
    }
}
