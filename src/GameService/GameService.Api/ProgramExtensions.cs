namespace Scrummy.GameService.Api;

public static class ProgramExtensions
{
    public static void AddCustomActors(this WebApplicationBuilder builder)
    {
        builder.Services.AddActors(options =>
        {
            options.Actors.RegisterActor<GameActor>();
            options.Actors.RegisterActor<LobbyActor>();
            options.Actors.RegisterActor<SessionActor>();
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

    public static void MapHubs(this IEndpointRouteBuilder builder)
    {
        builder.MapHub<GameHub>("/hub/gamehub", o => {
            o.Transports = HttpTransportType.WebSockets;
        });
    }
}
