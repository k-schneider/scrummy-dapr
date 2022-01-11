namespace Scrummy.GameService.Api.Controllers;

public class GameHub : Hub
{
    private readonly IActorProxyFactory _actorProxyFactory;

    public GameHub(IActorProxyFactory actorProxyFactory)
    {
        _actorProxyFactory = actorProxyFactory;
    }

    public override async Task OnConnectedAsync()
    {
        if (string.IsNullOrWhiteSpace(Context.UserIdentifier))
        {
            throw new Exception("Request does not contain a sid");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        var player = _actorProxyFactory.CreateActorProxy<IPlayerActor>(
            new ActorId(Context.UserIdentifier),
            typeof(PlayerActor).Name);

        await player.HandleDisconnect(Context.ConnectionId);

        await base.OnDisconnectedAsync(ex);
    }

    public async Task JoinGame(string gameId, string nickname)
    {
        var player = _actorProxyFactory.CreateActorProxy<IPlayerActor>(
            new ActorId(Context.UserIdentifier),
            typeof(PlayerActor).Name);

        await player.JoinGame(gameId, nickname, Context.ConnectionId);
    }

    public async Task LeaveGame(string gameId)
    {
        var player = _actorProxyFactory.CreateActorProxy<IPlayerActor>(
            new ActorId(Context.UserIdentifier),
            typeof(PlayerActor).Name);

        await player.LeaveGame(gameId);
    }
}
