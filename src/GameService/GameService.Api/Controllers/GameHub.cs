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
        var session = GetSessionActor();
        var gameId = await session.GetGameId();

        if (gameId is null)
        {
            throw new InvalidOperationException("Session is not associated with a game");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        await Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier!);
        await session.AddConnection(Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        await GetSessionActor().RemoveConnection(Context.ConnectionId);
        await base.OnDisconnectedAsync(ex);
    }

    private ISessionActor GetSessionActor()
    {
        if (string.IsNullOrEmpty(Context.UserIdentifier))
        {
            throw new UnauthorizedAccessException("Sid not provided");
        }

        return _actorProxyFactory.CreateActorProxy<ISessionActor>(
            new ActorId(Context.UserIdentifier),
            typeof(SessionActor).Name);
    }
}
