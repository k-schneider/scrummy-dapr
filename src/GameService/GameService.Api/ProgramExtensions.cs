// Only use in this file to avoid conflicts with Microsoft.Extensions.Logging
using Serilog;

namespace Scrummy.GameService.Api;

public static class ProgramExtensions
{
    private const string AppName = "Game Service";
    private const string AzureSignalRConnectionStringKey = "AzureSignalRConnectionString";

    public static void AddCustomActors(this WebApplicationBuilder builder)
    {
        builder.Services.AddActors(options =>
        {
            options.Actors.RegisterActor<GameActor>();
            options.Actors.RegisterActor<LobbyActor>();
            options.Actors.RegisterActor<PlayerActor>();
        });
    }

    public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEventBus, DaprEventBus>();
        builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
    }

    public static void AddCustomControllers(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<HttpResponseExceptionFilter>();
        });
    }

    public static void AddCustomHealthChecks(this WebApplicationBuilder builder) =>
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddDapr();

    public static void AddCustomSerilog(this WebApplicationBuilder builder)
    {
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console();

        var seqServerUrl = builder.Configuration["SeqServerUrl"];
        if (!string.IsNullOrWhiteSpace(seqServerUrl))
        {
            loggerConfig = loggerConfig
                .WriteTo.Seq(seqServerUrl);
        }

        Log.Logger = loggerConfig
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console()
            .Enrich.WithProperty("ApplicationName", AppName)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    public static void AddCustomSignalR(this WebApplicationBuilder builder)
    {
        var signalRBuilder = builder.Services
            .AddSignalR()
            .AddJsonProtocol();

        var azureSignalRConnectionString = builder.Configuration[AzureSignalRConnectionStringKey];
        if (!string.IsNullOrWhiteSpace(azureSignalRConnectionString))
        {
            signalRBuilder.AddAzureSignalR(options => {
                options.ConnectionString = azureSignalRConnectionString;
                options.TransportTypeDetector = (httpContext) =>
                    Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
            });
        }
    }

    public static void AddCustomSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = $"Scrummy - {AppName}", Version = "v1" });
        });
    }

    public static void UseSignalR(this WebApplication app)
    {
        app.MapHub<GameHub>("/hub/gamehub", o => {
            o.Transports = HttpTransportType.WebSockets;
        });
    }

    public static void UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{AppName} V1");
        });
    }
}
