using MicroserviceA.Class;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// ------------------
// Configure Serilog pour console locale (facultatif)
// ------------------
Serilog.Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.With(new OpenTelemetryEnricher()) // TraceId / SpanId
    .Enrich.WithProperty("ServiceName", "MicroserviceA")
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {ServiceName} TraceId: {TraceId} SpanId: {SpanId} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// ------------------
// Configure OpenTelemetry Logging pour OTLP Collector
// ------------------
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MicroserviceA"));
    logging.AddOtlpExporter(otlpOptions =>
    {
        otlpOptions.Endpoint = new Uri("http://otel-collector:4318/v1/logs"); // ton collector
        otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
    });
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
    logging.ParseStateValues = true;
});

// ------------------
// OpenTelemetry Tracing
// ------------------
//builder.Services.AddOpenTelemetryTracing(tracing =>
//{
//    tracing
//        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MicroserviceA"))
//        .AddAspNetCoreInstrumentation()
//        .AddHttpClientInstrumentation()
//        .AddOtlpExporter(otlpOptions =>
//        {
//            otlpOptions.Endpoint = new Uri("http://otel-collector:4318/v1/traces");
//            otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
//        });
//});

// ------------------
// Services / Swagger
// ------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();