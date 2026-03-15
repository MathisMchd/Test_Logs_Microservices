
using MicroserviceB.Class;
using OpenTelemetry.Logs;
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
        "[{Timestamp:HH:mm:ss} {Level:u3}] {ServiceName} TraceId: {TraceId} Span : {SpanId} {Message:lj}{NewLine}{Exception}") // CorId :{CorrelationId}
    .MinimumLevel.Information()
    .CreateLogger();
builder.Host.UseSerilog();


// ------------------
// OpenTelemetry Logging (logs applicatifs → OTLP)
// ------------------
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MicroserviceB"));
    logging.AddOtlpExporter(otlpOptions =>
    {
        otlpOptions.Endpoint = new Uri("http://otel-collector:4318/v1/logs");
        otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
    });

    logging.IncludeFormattedMessage = true; // message complet
    logging.IncludeScopes = true;            // contexte
    logging.ParseStateValues = true;         // propriétés
});
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
