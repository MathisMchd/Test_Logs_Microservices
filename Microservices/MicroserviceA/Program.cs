
using MicroserviceA.Class;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

// ------------------
// Configure Serilog avec Elastic.Serilog.Sinks
// ------------------

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.With(new OpenTelemetryEnricher())
    .Enrich.WithProperty("service.name", "MicroserviceA")
    .WriteTo.Console()
    .WriteTo.OpenTelemetry(options =>
    {
        options.Endpoint = "http://otel-collector:4318";
        options.Protocol = OtlpProtocol.HttpProtobuf;
    })
    .CreateLogger();

builder.Host.UseSerilog();



// ------------------
// OpenTelemetry Logging (logs applicatifs → OTLP)
// ------------------
//builder.Logging.AddOpenTelemetry(logging =>
//{
//    logging.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MicroserviceA"));
//    logging.AddOtlpExporter(otlpOptions =>
//    {
//        otlpOptions.Endpoint = new Uri("http://otel-collector:4318");
//        otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
//    });

//    logging.IncludeFormattedMessage = true; // message complet
//    logging.IncludeScopes = true;            // contexte
//    logging.ParseStateValues = true;         // propriétés
//});
// ------------------
// OpenTelemetry Tracing
// ------------------
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("MicroserviceA"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
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

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
