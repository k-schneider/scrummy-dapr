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
        // Gather up player votes and send to players
        var gameState = await GetGameActor(integrationEvent.GameId).GetGameState();

        var playerStates = await Task.WhenAll(gameState.Players.Select(p =>
            GetPlayerActor(p.Value).GetPlayerState(cancellationToken)));

        var votes = playerStates
            .Where(p => p.Vote is not null)
            .ToDictionary(p => p.PlayerId, p => p.Vote!);

        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.CardsFlipped,
                new CardsFlippedMessage(votes),
                cancellationToken);

        await GetGameActor(integrationEvent.GameId).SlideInactivityReminder(cancellationToken);
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

        // Remove game from lobby active game list
        await GetLobbyActor().NotifyGameEnded(
            integrationEvent.GameId,
            cancellationToken);

        // Clean up player and game actor state
        var game = GetGameActor(integrationEvent.GameId);
        var gameState = await game.GetGameState();
        await Task.WhenAll(gameState.Players.Select(p => GetPlayerActor(p.Value).Reset()));
        await game.Reset(cancellationToken);
    }

    [HttpPost("HostChanged")]
    [Topic(DAPR_PUBSUB_NAME, "HostChangedIntegrationEvent")]
    public async Task HandleAsync(HostChangedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(integrationEvent.GameId)
            .SendAsync(
                GameHubMethods.HostChanged,
                new HostChangedMessage(integrationEvent.PlayerId),
                cancellationToken);

        await GetGameActor(integrationEvent.GameId).SlideInactivityReminder(cancellationToken);
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

        await GetGameActor(integrationEvent.GameId).SlideInactivityReminder(cancellationToken);
    }

    [HttpPost("PlayerConnected")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerConnectedIntegrationEvent")]
    public async Task HandleAsync(PlayerConnectedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
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

        // Build a snapshot of the current game and players to send to the new connection
        var gameState = await GetGameActor(integrationEvent.GameId).GetGameState(cancellationToken);

        var playerStates = await Task.WhenAll(gameState.Players.Select(p =>
            GetPlayerActor(p.Value).GetPlayerState(cancellationToken)));

        var players = playerStates.Select(p => new Player(
            p.PlayerId,
            p.Nickname!,
            p.IsHost,
            p.ConnectionIds.Any(),
            p.IsSpectator,
            p.Sid == integrationEvent.Sid || gameState.GamePhase == GamePhases.Results ? p.Vote : null,
            p.Vote is not null));

        var game = new Game(
            gameState.GameId,
            gameState.GameVersion,
            gameState.GamePhase,
            players,
            gameState.Deck);

        await _hubContext.Clients
            .Client(integrationEvent.ConnectionId)
            .SendAsync(
                GameHubMethods.ReceiveGameState,
                new ReceiveGameStateMessage(game),
                cancellationToken);

        await GetGameActor(integrationEvent.GameId).SlideInactivityReminder(cancellationToken);
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

        await GetGameActor(integrationEvent.GameId).SlideInactivityReminder(cancellationToken);
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

        await GetGameActor(integrationEvent.GameId).SlideInactivityReminder(cancellationToken);
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

        if (integrationEvent.PlayerCount == 0)
        {
            await GetGameActor(integrationEvent.GameId).EndGame();
        }
        else if (integrationEvent.IsHost)
        {
            var gameState = await GetGameActor(integrationEvent.GameId).GetGameState(cancellationToken);

            if (gameState.Players.Any())
            {
                var newHostSid = gameState.Players.OrderBy(p => p.Key).First().Value;
                await GetPlayerActor(newHostSid).PromoteToHost();
            }
        }
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

        await GetGameActor(integrationEvent.GameId).SlideInactivityReminder(cancellationToken);
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

        await GetGameActor(integrationEvent.GameId).SlideInactivityReminder(cancellationToken);
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

        await GetGameActor(integrationEvent.GameId).SlideInactivityReminder(cancellationToken);
    }

    [HttpPost("PlayerVoteCast")]
    [Topic(DAPR_PUBSUB_NAME, "PlayerVoteCastIntegrationEvent")]
    public async Task HandleAsync(PlayerVoteCastIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var player = GetPlayerActor(integrationEvent.Sid);
        var playerState = await player.GetPlayerState(cancellationToken);
        var hadPreviousVote = integrationEvent.PreviousVote is not null;

        // Send players a notification that the player has voted excluding the vote value
        await _hubContext.Clients
            .GroupExcept(integrationEvent.GameId, playerState.ConnectionIds)
            .SendAsync(
                GameHubMethods.PlayerVoteCast,
                new PlayerVoteCastMessage(integrationEvent.PlayerId, hadPreviousVote, null),
                cancellationToken);

        // Send the player a notification to sync their vote across connections
        await _hubContext.Clients
            .Clients(playerState.ConnectionIds)
            .SendAsync(
                GameHubMethods.PlayerVoteCast,
                new PlayerVoteCastMessage(
                    integrationEvent.PlayerId, hadPreviousVote, integrationEvent.Vote),
                cancellationToken);

        await GetGameActor(integrationEvent.GameId).SlideInactivityReminder(cancellationToken);
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

        await GetGameActor(integrationEvent.GameId).SlideInactivityReminder(cancellationToken);
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

        await GetGameActor(integrationEvent.GameId).SlideInactivityReminder(cancellationToken);
    }

    private ILobbyActor GetLobbyActor() =>
        _actorProxyFactory.CreateActorProxy<ILobbyActor>(
            new ActorId(Guid.Empty.ToString()),
            typeof(LobbyActor).Name);

    private IGameActor GetGameActor(string gameId) =>
        _actorProxyFactory.CreateActorProxy<IGameActor>(
            new ActorId(gameId),
            typeof(GameActor).Name);

    private IPlayerActor GetPlayerActor(string sid) =>
        _actorProxyFactory.CreateActorProxy<IPlayerActor>(
            new ActorId(sid),
            typeof(PlayerActor).Name);
}
