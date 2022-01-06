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
                ClusterId = "gameapicluster",
                Match = new RouteMatch
                {
                    Path = "/game-api/{**catch-all}"
                },
                Transforms = new[]
                {
                    new Dictionary<string, string>
                    {
                        { "PathRemovePrefix", "/game-api" }
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
                ClusterId = "gameapicluster",
                Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                {
                    { "gameapicluster/destination1", new DestinationConfig { Address = builder.Configuration["GameApiUrl"] } }
                }
            }
        };

        builder.Services.AddReverseProxy()
            .LoadFromMemory(routes, clusters);
    }
}
