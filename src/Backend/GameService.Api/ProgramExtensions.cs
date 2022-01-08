namespace Scrummy.GameService.Api;

public static class ProgramExtensions
{
    public static void AddCustomActors(this WebApplicationBuilder builder)
    {
        builder.Services.AddActors(options =>
        {
            options.Actors.RegisterActor<GameActor>();
        });
    }

    public static void AddCustomUserIdProvider(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
    }

    public static void MapHubs(this IEndpointRouteBuilder builder)
    {
        builder.MapHub<GameHub>("/hub/gamehub", o => {
            o.Transports = HttpTransportType.WebSockets;
        });
    }
}
