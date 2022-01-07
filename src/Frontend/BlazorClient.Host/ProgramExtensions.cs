namespace Scrummy.BlazorClient.Host;

public static class ProgramExtensions
{
    public static void AddCustomReverseProxy(this WebApplicationBuilder builder)
    {
        var routes = new[]
        {
            new RouteConfig
            {
                RouteId = "gameservice",
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
                    },
                    new Dictionary<string, string>
                    {
                        { "X-Forwarded", "Append" },
                        { "HeaderPrefix", "X-Forwarded-" }
                    },
                    new Dictionary<string, string>
                    {
                        { "Forwarded", "by,host,for,proto" },
                        { "ByFormat", "Random" },
                        { "ForFormat", "IpAndPort" }
                    },
                    new Dictionary<string, string>
                    {
                        { "RequestHeadersCopy", "true" }
                    },
                    new Dictionary<string, string>
                    {
                        { "RequestHeaderOriginalHost", "true" }
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
