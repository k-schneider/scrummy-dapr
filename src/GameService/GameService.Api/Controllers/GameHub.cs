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
        var player = GetPlayerActor();
        var playerState = await player.GetPlayerState();

        if (playerState.GameId is null)
        {
            throw new InvalidOperationException("Player has not joined a game");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, playerState.GameId);
        await Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier!);
        await player.AddConnection(Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        await GetPlayerActor().RemoveConnection(Context.ConnectionId);
        await base.OnDisconnectedAsync(ex);
    }

    private IPlayerActor GetPlayerActor()
    {
        if (string.IsNullOrEmpty(Context.UserIdentifier))
        {
            throw new UnauthorizedAccessException("Sid not provided");
        }

        return _actorProxyFactory.CreateActorProxy<IPlayerActor>(
            new ActorId(Context.UserIdentifier),
            typeof(PlayerActor).Name);
    }
}
