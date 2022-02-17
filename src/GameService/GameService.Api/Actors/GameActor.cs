namespace Scrummy.GameService.Api.Actors;

public class GameActor : Actor, IGameActor , IRemindable
{
    private const string GameStateName = "GameState";
    private const string InactivityReminderName = "InactivityReminder";

    private readonly IEventBus _eventBus;

    private GameState _gameState = null!;

    private string GameId => Id.GetId();

    public GameActor(ActorHost host, IEventBus eventBus)
        : base(host)
    {
        _eventBus = eventBus;
    }

    protected override async Task OnActivateAsync()
    {
        var gameState = await StateManager.TryGetStateAsync<GameState>(GameStateName);
        _gameState = gameState.HasValue ? gameState.Value : new GameState(GameId);

        await base.OnActivateAsync();
    }

    public async Task<(int PlayerId, int PlayerCount)> AddPlayer(string sid, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        var playerId = ++_gameState.PlayerCounter;

        _gameState.Players.Add((sid, playerId));

        await SaveGameState(cancellationToken);

        return (playerId,  _gameState.Players.Count);
    }

    public async Task BeginVoting(CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        _gameState.GamePhase = GamePhases.Voting;

        await SaveGameState(cancellationToken);
    }

    public async Task EndGame(CancellationToken cancellationToken = default)
    {
        if (_gameState.GameStatus == GameStatuses.InProgress)
        {
            _gameState.GameStatus = GameStatuses.GameOver;

            await SaveGameState(cancellationToken);

            await StopInactivityTimer(cancellationToken);

            await _eventBus.PublishAsync(new GameEndedIntegrationEvent(GameId));
        }
    }

    public Task<GameState> GetGameState(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_gameState);
    }

    public async Task<int> RemovePlayer(string sid, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        var player = _gameState.Players.FirstOrDefault(p => p.Sid == sid);

        if (player == default)
        {
            throw new InvalidOperationException("Player is not in the game");
        }

        _gameState.Players.Remove(player);

        await SaveGameState(cancellationToken);

        return _gameState.Players.Count;
    }

    public async Task Reset(CancellationToken cancellationToken = default)
    {
        await StateManager.TryRemoveStateAsync(GameStateName, cancellationToken);
        _gameState = new(GameId);
    }

    public async Task ShowResults(CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        _gameState.GamePhase = GamePhases.Results;

        await SaveGameState(cancellationToken);
    }

    public async Task SlideInactivityReminder(CancellationToken cancellationToken = default)
    {
        if (_gameState.GameStatus == GameStatuses.InProgress)
        {
            await StopInactivityTimer(cancellationToken);
            await StartInactivityTimer(cancellationToken);
        }
    }

    public async Task StartGame(IEnumerable<Card> deck, CancellationToken cancellationToken = default)
    {
        if (_gameState.GameStatus != GameStatuses.None)
        {
            throw new InvalidOperationException("Game has already started");
        }

        _gameState.GameStatus = GameStatuses.InProgress;
        _gameState.Deck.AddRange(deck);

        await SaveGameState(cancellationToken);

        await StartInactivityTimer(cancellationToken);

        await _eventBus.PublishAsync(
            new GameStartedIntegrationEvent(GameId),
            cancellationToken);
    }

    async Task IRemindable.ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        if (reminderName == InactivityReminderName)
        {
            await EndGame();
        }
    }

    private void EnsureGameInProgress(string error = "Game is not in progress")
    {
        if (_gameState.GameStatus != GameStatuses.InProgress)
        {
            throw new InvalidOperationException(error);
        }
    }

    private Task SaveGameState(CancellationToken cancellationToken = default) =>
        StateManager.SetStateAsync(GameStateName, _gameState, cancellationToken);

    private Task StartInactivityTimer(CancellationToken cancellationToken = default) =>
        RegisterReminderAsync(
            InactivityReminderName,
            Array.Empty<byte>(),
            TimeSpan.FromDays(1),
            TimeSpan.FromMilliseconds(-1));

    private Task StopInactivityTimer(CancellationToken cancellationToken = default) =>
        UnregisterReminderAsync(InactivityReminderName);
}
