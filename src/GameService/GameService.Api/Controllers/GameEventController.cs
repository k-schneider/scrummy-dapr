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

    [HttpPost("CardsFlipped")]
    [Topic(DAPR_PUBSUB_NAME, "CardsFlippedIntegrationEvent")]
    public async Task HandleAsync(CardsFlippedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.CardsFlipped,
                new CardsFlippedMessage(integrationEvent.Votes),
                cancellationToken);
    }

    [HttpPost("GameEnded")]
    [Topic(DAPR_PUBSUB_NAME, "GameEndedIntegrationEvent")]
    public async Task HandleAsync(GameEndedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await GetLobbyActor().NotifyGameEnded(integrationEvent.GameId, cancellationToken);
    }

    [HttpPost("HostChanged")]
    [Topic(DAPR_PUBSUB_NAME, "HostChangedIntegrationEvent")]
    public async Task HandleAsync(HostChangedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.HostChanged,
                new HostChangedMessage(integrationEvent.NewHostPlayerId),
                cancellationToken);
    }

    [HttpPost("NewVoteStarted")]
    [Topic(DAPR_PUBSUB_NAME, "NewVoteStartedIntegrationEvent")]
    public async Task HandleAsync(NewVoteStartedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.NewVoteStarted,
                cancellationToken);
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
                new SyncGameMessage(await game.GetGameSnapshot(integrationEvent.PlayerId, cancellationToken)),
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

    [HttpPost("PlayerVoteCast")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerVoteCastIntegrationEvent")]
    public async Task HandleAsync(PlayerVoteCastIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var session = GetSessionActor(integrationEvent.Sid);
        var connectionIds = await session.GetConnectionIds(cancellationToken);

        // Send players a notification that the player has voted excluding the vote value
        await _hubContext.Clients
            .GroupExcept(integrationEvent.GameId, connectionIds)
            .SendAsync(
                GameHubMethods.PlayerVoteCast,
                new PlayerVoteCastMessage(integrationEvent.PlayerId, null),
                cancellationToken);

        // Send the player a notification to sync their vote across connections
        await _hubContext.Clients
            .Clients(connectionIds)
            .SendAsync(
                GameHubMethods.PlayerVoteCast,
                new PlayerVoteCastMessage(integrationEvent.PlayerId, integrationEvent.Vote),
                cancellationToken);
    }

    [HttpPost("PlayerVoteRecalled")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerVoteRecalledIntegrationEvent")]
    public async Task HandleAsync(PlayerVoteRecalledIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        // Send players a notification that the player has recalled their vote
        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.PlayerVoteRecalled,
                new PlayerVoteRecalledMessage(integrationEvent.PlayerId),
                cancellationToken);
    }

    [HttpPost("VotesReset")]
    [Topic(DAPR_PUBSUB_NAME, "VotesResetIntegrationEvent")]
    public async Task HandleAsync(VotesResetIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.VotesReset,
                cancellationToken);
    }

    private IGameActor GetGameActor(string gameId) =>
        _actorProxyFactory.CreateActorProxy<IGameActor>(
            new ActorId(gameId),
            typeof(GameActor).Name);

    private ILobbyActor GetLobbyActor() =>
        _actorProxyFactory.CreateActorProxy<ILobbyActor>(
            new ActorId(Guid.Empty.ToString()),
            typeof(LobbyActor).Name);

    private ISessionActor GetSessionActor(string sid) =>
        _actorProxyFactory.CreateActorProxy<ISessionActor>(
            new ActorId(sid),
            typeof(SessionActor).Name);
}
