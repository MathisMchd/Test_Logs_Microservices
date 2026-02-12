using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using MicroserviceB.Class;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ------------------
// Configure Serilog avec Elastic.Serilog.Sinks
// ------------------
Serilog.Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()               // CorrelationId middleware
    .Enrich.With(new OpenTelemetryEnricher()) // TraceId / SpanId
    .Enrich.WithProperty("ServiceName", "MicroserviceB")
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {ServiceName} {CorrelationId} {TraceId} {SpanId} {Message:lj}{NewLine}{Exception}")
    .WriteTo.Elasticsearch(
        new[] { new Uri("http://elasticsearch:9200") },
        options =>
        {
            options.DataStream = new DataStreamName("logs", "microservice-b", "default");
            options.BootstrapMethod = BootstrapMethod.Failure;
        },
        transport =>
        {
            // transport.Authentication(new BasicAuthentication("user","pass"));
        }
    )
    .MinimumLevel.Information()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddHttpClient("ServiceBClient")
    .AddHttpMessageHandler<TracePropagationHandler>();
// ------------------
// OpenTelemetry Tracing
// ------------------
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("MicroserviceB"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
        // .AddOtlpExporter(); // si tu veux envoyer vers OTEL collector
    });



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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
