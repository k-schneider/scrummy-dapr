namespace Scrummy.GameService.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class GameEventController : ControllerBase
{
    private const string DAPR_PUBSUB_NAME = "pubsub";

    private readonly IActorProxyFactory _actorProxyFactory;
    private readonly IHubContext<GameHub> _hubContext;

    public GameEventController(
        IActorProxyFactory actorProxyFactory,
        IHubContext<GameHub> hubContext)
    {
        _actorProxyFactory = actorProxyFactory;
        _hubContext = hubContext;
    }

    [HttpPost("PlayerConnected")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerConnectedIntegrationEvent")]
    public async Task HandleAsync(PlayerConnectedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var game = GetGameActor(integrationEvent.GameId);

        await game.NotifyPlayerConnected(integrationEvent.PlayerId, cancellationToken);

        // Send the new connection a snapshot of the current game state
        await _hubContext.Clients
            .Client(integrationEvent.ConnectionId)
            .SendAsync(
                GameHubMethods.SyncGame,
                new SyncGameMessage(await game.GetGameSnapshot(cancellationToken)),
                cancellationToken);

        if (integrationEvent.ConnectionCount == 1)
        {
            // Let everyone in the game know this player is now online
            await _hubContext.Clients
                .GroupExcept(integrationEvent.GameId, integrationEvent.ConnectionId)
                .SendAsync(
                    GameHubMethods.PlayerConnected,
                    new PlayerConnectedMessage(integrationEvent.PlayerId),
                    cancellationToken);
        }
    }

    [HttpPost("PlayerDisconnected")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerDisconnectedIntegrationEvent")]
    public async Task HandleAsync(PlayerDisconnectedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (integrationEvent.ConnectionCount == 0)
        {
            // Let everyone in the game know this player is now offline
            await _hubContext.Clients
                .GroupExcept(integrationEvent.GameId, integrationEvent.ConnectionId)
                .SendAsync(
                    GameHubMethods.PlayerDisconnected,
                    new PlayerDisconnectedMessage(integrationEvent.PlayerId),
                    cancellationToken);
        }
    }

    [HttpPost("PlayerJoinedGame")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerJoinedGameIntegrationEvent")]
    public async Task HandleAsync(PlayerJoinedGameIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.PlayerJoinedGame,
                new PlayerJoinedGameMessage(integrationEvent.PlayerId, integrationEvent.Nickname),
                cancellationToken);
    }

    [HttpPost("PlayerLeftGame")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerLeftGameIntegrationEvent")]
    public async Task HandleAsync(PlayerLeftGameIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.PlayerLeftGame,
                new PlayerLeftGameMessage(integrationEvent.PlayerId),
                cancellationToken);
    }

    private IGameActor GetGameActor(string gameId) =>
        _actorProxyFactory.CreateActorProxy<IGameActor>(
            new ActorId(gameId),
            typeof(GameActor).Name);
}
