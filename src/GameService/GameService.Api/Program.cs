var appName = "Game Service";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddCustomSignalR();
builder.AddCustomSerilog();
builder.AddCustomSwagger();
builder.AddCustomHealthChecks();
builder.AddCustomActors();
builder.AddCustomApplicationServices();
builder.AddCustomControllers();

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddDaprClient();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpLogging();
app.MapGet("/", () => Results.LocalRedirect("~/swagger"));
app.UseCustomSwagger();
app.UseCloudEvents();
app.UseAuthorization();
app.UseSignalR();
app.MapActorsHandlers();
app.MapControllers();
app.MapSubscribeHandler();
app.MapHealthChecks("/hc", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});

try
{
    app.Logger.LogInformation("Starting web host ({ApplicationName})...", appName);
    app.Run();
    return 0;
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly ({ApplicationName})...", appName);
    return 1;
}
finally
{
    Serilog.Log.CloseAndFlush();
}
