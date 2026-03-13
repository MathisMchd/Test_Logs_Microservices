using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using MicroserviceB.Class;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// ------------------
// Configure Serilog avec Elastic.Serilog.Sinks
// ------------------
Serilog.Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()               // CorrelationId middleware
    .Enrich.With(new OpenTelemetryEnricher()) // TraceId / SpanId
    .Enrich.WithProperty("ServiceName", "MicroserviceB")
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // ignore les logs info Microsoft
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {ServiceName} TraceId: {TraceId} Span : {SpanId} {Message:lj}{NewLine}{Exception}") // CorId :{CorrelationId}
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


// ------------------
// OpenTelemetry Tracing
// ------------------
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("MicroserviceB"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
        // .AddOtlpExporter(); // si tu veux envoyer vers OTEL collector
    });

builder.Services.AddHttpClient();


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

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
