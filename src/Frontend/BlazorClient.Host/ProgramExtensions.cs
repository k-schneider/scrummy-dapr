namespace Scrummy.BlazorClient.Host;

public static class ProgramExtensions
{
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
}
