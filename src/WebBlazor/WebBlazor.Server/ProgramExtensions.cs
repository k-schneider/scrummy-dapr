// Only use in this file to avoid conflicts with Microsoft.Extensions.Logging
using Serilog;

namespace Scrummy.WebBlazor.Server;

public static class ProgramExtensions
{
    private const string AppName = "Web Blazor";

    public static void AddCustomHealthChecks(this WebApplicationBuilder builder) =>
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());

    public static void AddCustomReverseProxy(this WebApplicationBuilder builder)
    {
        var routes = new[]
        {
            new RouteConfig
            {
                RouteId = "gameapi",
                ClusterId = "gameservicecluster",
                Match = new RouteMatch
                {
                    Path = "/g/{**catch-all}"
                },
                Transforms = new[]
                {
                    new Dictionary<string, string>
                    {
                        { "PathRemovePrefix", "/g" }
                    },
                    new Dictionary<string, string>
                    {
                        { "PathPrefix", "/api" }
                    }
                }
            },
            new RouteConfig
            {
                RouteId = "gamehub",
                ClusterId = "gameservicecluster",
                Match = new RouteMatch
                {
                    Path = "/h/{**catch-all}"
                },
                Transforms = new[]
                {
                    new Dictionary<string, string>
                    {
                        { "PathRemovePrefix", "/h" }
                    },
                    new Dictionary<string, string>
                    {
                        { "PathPrefix", "/hub" }
                    }
                }
            }
        };

        var clusters = new[]
        {
            new ClusterConfig
            {
                ClusterId = "gameservicecluster",
                Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                {
                    { "gameservicecluster/destination1", new DestinationConfig { Address = builder.Configuration["GameServiceUrl"] } }
                }
            }
        };

        builder.Services.AddReverseProxy()
            .LoadFromMemory(routes, clusters);
    }

    public static void AddCustomSerilog(this WebApplicationBuilder builder)
    {
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console();

        var seqServerUrl = builder.Configuration["SeqServerUrl"];
        if (!string.IsNullOrWhiteSpace(seqServerUrl))
        {
            loggerConfig = loggerConfig
                .WriteTo.Seq(seqServerUrl)
                .Enrich.WithProperty("ApplicationName", AppName);
        }

        var azureAnalyticsWorkspaceId = builder.Configuration["AzureAnalyticsWorkspaceId"];
        var azureAnalyticsPrimaryKey = builder.Configuration["AzureAnalyticsPrimaryKey"];
        if (!string.IsNullOrWhiteSpace(azureAnalyticsWorkspaceId) &&
            !string.IsNullOrWhiteSpace(azureAnalyticsPrimaryKey))
        {
            loggerConfig = loggerConfig.WriteTo.AzureAnalytics(
                azureAnalyticsWorkspaceId,
                azureAnalyticsPrimaryKey,
                AppName.Replace(" ", string.Empty));
        }

        Log.Logger = loggerConfig
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console()
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}
