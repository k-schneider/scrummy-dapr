namespace Scrummy.GameService.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class GameEventController : ControllerBase
{
    private readonly IActorProxyFactory _actorProxyFactory;
    private readonly IHubContext<GameHub> _hubContext;

    public GameEventController(
        IActorProxyFactory actorProxyFactory,
        IHubContext<GameHub> hubContext)
    {
        _actorProxyFactory = actorProxyFactory;
        _hubContext = hubContext;
    }


    [HttpPost("PlayerConnectedEvent")]
    [Topic(Constants.DaprPubSubName, "PlayerConnectedEvent")]
    public async Task HandleAsync(PlayerConnectedEvent integrationEvent, CancellationToken cancellationToken)
    {
        // Send the new connection a snapshot of the current game state
        await _hubContext.Clients
            .Client(integrationEvent.ConnectionId)
            .SendAsync(
                "Sync",
                new
                {
                    Game = await GetGameActor(integrationEvent.GameId).GetGameSnapshot(cancellationToken)
                },
                cancellationToken);

        if (integrationEvent.ConnectionCount == 1)
        {
            // Let everyone know this player is now online
            await _hubContext.Clients
                .GroupExcept(integrationEvent.GameId, integrationEvent.ConnectionId)
                .SendAsync(
                    "PlayerConnected",
                    new
                    {
                        PlayerId = integrationEvent.PlayerId
                    },
                    cancellationToken);
        }
    }

    [HttpPost("PlayerDisconnectedEvent")]
    [Topic(Constants.DaprPubSubName, "PlayerDisconnectedEvent")]
    public async Task HandleAsync(PlayerDisconnectedEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (integrationEvent.ConnectionCount == 0)
        {
            // Let everyone know this player is now offline
            await _hubContext.Clients
                .GroupExcept(integrationEvent.GameId, integrationEvent.ConnectionId)
                .SendAsync(
                    "PlayerDisconnected",
                    new
                    {
                        PlayerId = integrationEvent.PlayerId
                    },
                    cancellationToken);
        }
    }

    private IGameActor GetGameActor(string gameId) =>
        _actorProxyFactory.CreateActorProxy<IGameActor>(
            new ActorId(gameId),
            typeof(GameActor).Name);
}
