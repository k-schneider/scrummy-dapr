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
        // Send the new connection a snapshot of the current game state
        await _hubContext.Clients
            .Client(integrationEvent.ConnectionId)
            .SendAsync(
                "SyncGame",
                new SyncGameMessage(
                    await GetGameActor(integrationEvent.GameId)
                        .GetGameSnapshot(cancellationToken)),
                cancellationToken);

        if (integrationEvent.ConnectionCount == 1)
        {
            // Let everyone in the game know this player is now online
            await _hubContext.Clients
                .GroupExcept(integrationEvent.GameId, integrationEvent.ConnectionId)
                .SendAsync(
                    "PlayerConnected",
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
                    "PlayerDisconnected",
                    new PlayerDisconnectedMessage(integrationEvent.PlayerId),
                    cancellationToken);
        }
    }

    private IGameActor GetGameActor(string gameId) =>
        _actorProxyFactory.CreateActorProxy<IGameActor>(
            new ActorId(gameId),
            typeof(GameActor).Name);
}
