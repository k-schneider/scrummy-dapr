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
        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.GameEnded,
                cancellationToken);

        await GetLobbyActor().NotifyGameEnded(
            integrationEvent.GameId,
            cancellationToken);

        await GetGameActor(integrationEvent.GameId)
            .ResetGame(cancellationToken);
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

        if (integrationEvent.ConnectionCount == 1)
        {
            await game.NotifyPlayerConnected(integrationEvent.PlayerId, cancellationToken);

            // Let everyone in the game know this player is now online
            await _hubContext.Clients
                .GroupExcept(integrationEvent.GameId, integrationEvent.ConnectionId)
                .SendAsync(
                    GameHubMethods.PlayerConnected,
                    new PlayerConnectedMessage(integrationEvent.PlayerId),
                    cancellationToken);
        }

        // Send the new connection the games current state
        await _hubContext.Clients
            .Client(integrationEvent.ConnectionId)
            .SendAsync(
                GameHubMethods.ReceiveGameState,
                new ReceiveGameStateMessage(await game.GetGameState(integrationEvent.PlayerId, cancellationToken)),
                cancellationToken);
    }

    [HttpPost("PlayerDisconnected")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerDisconnectedIntegrationEvent")]
    public async Task HandleAsync(PlayerDisconnectedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (integrationEvent.ConnectionCount == 0)
        {
            var game = GetGameActor(integrationEvent.GameId);
            await game.NotifyPlayerDisconnected(integrationEvent.PlayerId, cancellationToken);

            // Let everyone in the game know this player is now offline
            await _hubContext.Clients
                .GroupExcept(integrationEvent.GameId, integrationEvent.ConnectionId)
                .SendAsync(
                    GameHubMethods.PlayerDisconnected,
                    new PlayerDisconnectedMessage(integrationEvent.PlayerId),
                    cancellationToken);
        }
    }

    [HttpPost("PlayerIsSpectatorChanged")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerIsSpectatorChangedIntegrationEvent")]
    public async Task HandleAsync(PlayerIsSpectatorChangedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.PlayerIsSpectatorChanged,
                new PlayerIsSpectatorChangedMessage(integrationEvent.PlayerId, integrationEvent.IsSpectator),
                cancellationToken);
    }

    [HttpPost("PlayerJoined")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerJoinedIntegrationEvent")]
    public async Task HandleAsync(PlayerJoinedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.PlayerJoined,
                new PlayerJoinedMessage(integrationEvent.PlayerId, integrationEvent.Nickname),
                cancellationToken);
    }

    [HttpPost("PlayerLeft")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerLeftIntegrationEvent")]
    public async Task HandleAsync(PlayerLeftIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.PlayerLeft,
                new PlayerLeftMessage(integrationEvent.PlayerId),
                cancellationToken);
    }

    [HttpPost("PlayerNicknameChanged")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerNicknameChangedIntegrationEvent")]
    public async Task HandleAsync(PlayerNicknameChangedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.PlayerNicknameChanged,
                new PlayerNicknameChangedMessage(integrationEvent.PlayerId, integrationEvent.Nickname),
                cancellationToken);
    }

    [HttpPost("PlayerNudged")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerNudgedIntegrationEvent")]
    public async Task HandleAsync(PlayerNudgedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.PlayerNudged,
                new PlayerNudgedMessage(integrationEvent.FromPlayerId, integrationEvent.ToPlayerId),
                cancellationToken);
    }

    [HttpPost("PlayerRemoved")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerRemovedIntegrationEvent")]
    public async Task HandleAsync(PlayerRemovedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.PlayerRemoved,
                new PlayerRemovedMessage(integrationEvent.PlayerId),
                cancellationToken);
    }

    [HttpPost("PlayerVoteCast")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerVoteCastIntegrationEvent")]
    public async Task HandleAsync(PlayerVoteCastIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var session = GetSessionActor(integrationEvent.Sid);
        var connectionIds = await session.GetConnectionIds(cancellationToken);
        var hadPreviousVote = integrationEvent.PreviousVote is not null;

        // Send players a notification that the player has voted excluding the vote value
        await _hubContext.Clients
            .GroupExcept(integrationEvent.GameId, connectionIds)
            .SendAsync(
                GameHubMethods.PlayerVoteCast,
                new PlayerVoteCastMessage(integrationEvent.PlayerId, hadPreviousVote, null),
                cancellationToken);

        // Send the player a notification to sync their vote across connections
        await _hubContext.Clients
            .Clients(connectionIds)
            .SendAsync(
                GameHubMethods.PlayerVoteCast,
                new PlayerVoteCastMessage(
                    integrationEvent.PlayerId, hadPreviousVote, integrationEvent.Vote),
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
