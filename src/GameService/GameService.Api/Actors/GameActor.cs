namespace Scrummy.GameService.Api.Actors;

public class GameActor : Actor, IGameActor, IRemindable
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
        _gameState = gameState.HasValue ? gameState.Value : new GameState();

        await base.OnActivateAsync();
    }

    public async Task<(string sid, int playerId)> AddPlayer(string nickname, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        var sid = NewSid();
        var playerId = ++_gameState.PlayerCounter;

        await GetSessionActor(sid).AssociateWithGame(GameId, playerId, cancellationToken);

        _gameState.Players.Add(new PlayerState
        {
            Sid = sid,
            PlayerId = playerId,
            Nickname = nickname,
            IsHost = !_gameState.Players.Any()
        });

        await SaveGameState(cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerJoinedIntegrationEvent(
                sid,
                playerId,
                nickname,
                GameId),
            cancellationToken);

        await SetInactivityReminder(cancellationToken);

        return (sid, playerId);
    }

    public async Task CastVote(string sid, string vote, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        if (!_gameState.Deck.Any(c => c.Id == vote))
        {
            throw new InvalidOperationException("Invalid vote");
        }

        var self = GetRequiredPlayer(sid);

        if (self.IsSpectator)
        {
            throw new InvalidOperationException("Spectators cannot vote");
        }

        _gameState.Votes.TryGetValue(self.PlayerId, out var previousVote);
        _gameState.Votes[self.PlayerId] = vote;

        await SaveGameState(cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerVoteCastIntegrationEvent(
                self.Sid,
                self.PlayerId,
                vote,
                previousVote,
                GameId),
            cancellationToken);

        await SetInactivityReminder(cancellationToken);
    }

    public async Task FlipCards(string sid, CancellationToken cancellationToken = default)
    {
        EnsureVotingPhase();
        EnsureHost(sid, "Only host can flip cards");

        _gameState.GamePhase = GamePhases.Results;

        await SaveGameState(cancellationToken);

        await _eventBus.PublishAsync(
            new CardsFlippedIntegrationEvent(GameId, _gameState.Votes),
            cancellationToken);

        await SetInactivityReminder(cancellationToken);
    }

    public Task<Game> GetGameState(int playerId, CancellationToken cancellationToken = default)
    {
        var players = _gameState.Players.Select(p => new Player(
            p.PlayerId,
            p.Nickname,
            p.IsHost,
            p.IsConnected,
            p.IsSpectator)).ToList();

        var votes = _gameState.Votes.ToDictionary(kvp =>
            kvp.Key,
            // only return vote values for other players when showing results
            kvp => kvp.Key == playerId || _gameState.GamePhase == GamePhases.Results ? kvp.Value : null);

        return Task.FromResult(new Game(
            GameId,
            _gameState.GameVersion,
            _gameState.GamePhase,
            players,
            _gameState.Deck,
            votes));
    }

    public async Task LeaveGame(string sid, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();
        var self = GetRequiredPlayer(sid);

        _gameState.Players.Remove(self);
        _gameState.Votes.Remove(self.PlayerId);

        await SaveGameState(cancellationToken);

        await GetSessionActor(self.Sid).Reset(cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerLeftIntegrationEvent(
                self.Sid,
                self.PlayerId,
                GameId),
            cancellationToken);

        // If the player was the host, pick a new host based on when they joined
        if (self.IsHost && _gameState.Players.Any())
        {
            var newHost = _gameState.Players.OrderBy(p => p.JoinDate).First();
            newHost.IsHost = true;

            await SaveGameState(cancellationToken);

            await _eventBus.PublishAsync(
                new HostChangedIntegrationEvent(
                    self.Sid,
                    self.PlayerId,
                    self.Nickname,
                    newHost.Sid,
                    newHost.PlayerId,
                    newHost.Nickname,
                    GameId),
                cancellationToken);
        }

        if (!_gameState.Players.Any())
        {
            _gameState.GameStatus = GameStatuses.GameOver;

            await SaveGameState(cancellationToken);

            await _eventBus.PublishAsync(
                new GameEndedIntegrationEvent(GameId),
                cancellationToken);
        }

        await SetInactivityReminder(cancellationToken);
    }

    public async Task NudgePlayer(string sid, int playerId, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        var self = GetRequiredPlayer(sid);
        var player = GetRequiredPlayer(playerId);

        await _eventBus.PublishAsync(
            new PlayerNudgedIntegrationEvent(
                self.Sid,
                self.PlayerId,
                player.Sid,
                player.PlayerId,
                GameId),
            cancellationToken);

        await SetInactivityReminder(cancellationToken);
    }

    public async Task NotifyPlayerConnected(int playerId, CancellationToken cancellationToken = default)
    {
        var player = _gameState.Players
            .FirstOrDefault(p => p.PlayerId == playerId);

        if (player is not null)
        {
            player.IsConnected = true;
            await SaveGameState(cancellationToken);
        }

        await SetInactivityReminder(cancellationToken);
    }

    public async Task NotifyPlayerDisconnected(int playerId, CancellationToken cancellationToken = default)
    {
        var player = _gameState.Players
            .FirstOrDefault(p => p.PlayerId == playerId);

        if (player is not null)
        {
            player.IsConnected = false;
            await SaveGameState(cancellationToken);
        }

        await SetInactivityReminder(cancellationToken);
    }

    public async Task PlayAgain(string sid, CancellationToken cancellationToken = default)
    {
        EnsureResultsPhase();
        EnsureHost(sid, "Only host can choose to play again");

        _gameState.Votes.Clear();
        _gameState.GamePhase = GamePhases.Voting;

        await SaveGameState(cancellationToken);

        await _eventBus.PublishAsync(
            new NewVoteStartedIntegrationEvent(GameId),
            cancellationToken);

        await SetInactivityReminder(cancellationToken);
    }

    public async Task PromotePlayer(string sid, int playerId, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();
        EnsureHost(sid, "Only host can promote other players");

        var self = GetRequiredPlayer(sid);
        var newHost = GetRequiredPlayer(playerId);

        self.IsHost = false;
        newHost.IsHost = true;

        await SaveGameState(cancellationToken);

        await _eventBus.PublishAsync(
            new HostChangedIntegrationEvent(
                self.Sid,
                self.PlayerId,
                self.Nickname,
                newHost.Sid,
                newHost.PlayerId,
                newHost.Nickname,
                GameId),
            cancellationToken);

        await SetInactivityReminder(cancellationToken);
    }

    public async Task RecallVote(string sid, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        var player = GetRequiredPlayer(sid);

        _gameState.Votes.TryGetValue(player.PlayerId, out var previousVote);
        _gameState.Votes.Remove(player.PlayerId);

        await SaveGameState(cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerVoteRecalledIntegrationEvent(
                player.Sid,
                player.PlayerId,
                previousVote,
                GameId),
            cancellationToken);

        await SetInactivityReminder(cancellationToken);
    }

    public async Task RemovePlayer(string sid, int playerId, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();
        EnsureHost(sid, "Only host can remove players");

        var self = GetRequiredPlayer(sid);

        if (self.PlayerId == playerId)
        {
            throw new InvalidOperationException("Cannot remove yourself");
        }

        var player = GetRequiredPlayer(playerId);

        _gameState.Players.Remove(player);
        _gameState.Votes.Remove(player.PlayerId);

        await SaveGameState(cancellationToken);

        await GetSessionActor(player.Sid).Reset(cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerRemovedIntegrationEvent(
                player.Sid,
                player.PlayerId,
                GameId),
            cancellationToken);

        await SetInactivityReminder();
    }

    public async Task ResetGame(CancellationToken cancellationToken = default)
    {
        if (_gameState.GameStatus != GameStatuses.GameOver)
        {
            throw new InvalidOperationException("Game must be over to reset");
        }

        await Task.WhenAll(_gameState.Players.Select(p => GetSessionActor(p.Sid).Reset()));
        await StateManager.TryRemoveStateAsync(GameStateName, cancellationToken);
        _gameState = new();
    }

    public async Task ResetVotes(string sid, CancellationToken cancellationToken = default)
    {
        EnsureVotingPhase();
        EnsureHost(sid, "Only host can reset votes");

        _gameState.Votes.Clear();

        await SaveGameState(cancellationToken);

        await _eventBus.PublishAsync(
            new VotesResetIntegrationEvent(GameId),
            cancellationToken);

        await SetInactivityReminder();
    }

    public async Task StartGame(CancellationToken cancellationToken = default)
    {
        if (_gameState.GameStatus != GameStatuses.None)
        {
            throw new InvalidOperationException("Game has already started");
        }

        _gameState.GameStatus = GameStatuses.InProgress;

        await SaveGameState(cancellationToken);

        await _eventBus.PublishAsync(
            new GameStartedIntegrationEvent(GameId),
            cancellationToken);

        await SetInactivityReminder();
    }

    public async Task StartSpectating(string sid, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        var self = GetRequiredPlayer(sid);

        self.IsSpectator = true;
        _gameState.Votes.Remove(self.PlayerId);

        await SaveGameState(cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerIsSpectatorChangedIntegrationEvent(
                self.Sid,
                self.PlayerId,
                self.IsSpectator,
                GameId),
            cancellationToken);

        await SetInactivityReminder(cancellationToken);
    }

    public async Task StopSpectating(string sid, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        var self = GetRequiredPlayer(sid);

        self.IsSpectator = false;

        await SaveGameState(cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerIsSpectatorChangedIntegrationEvent(
                self.Sid,
                self.PlayerId,
                self.IsSpectator,
                GameId),
            cancellationToken);

        await SetInactivityReminder(cancellationToken);
    }

    public async Task UpdateNickname(string sid, string nickname, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        var self = GetRequiredPlayer(sid);

        self.Nickname = nickname;

        await SaveGameState(cancellationToken);

        await _eventBus.PublishAsync(
            new PlayerNicknameChangedIntegrationEvent(
                self.Sid,
                self.PlayerId,
                nickname,
                GameId),
            cancellationToken);

        await SetInactivityReminder(cancellationToken);
    }

    async Task IRemindable.ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        if (reminderName == InactivityReminderName)
        {
            _gameState.GameStatus = GameStatuses.GameOver;

            await SaveGameState();

            await _eventBus.PublishAsync(new GameEndedIntegrationEvent(GameId));
        }
    }

    private string NewSid() => Guid.NewGuid().ToString();

    private void EnsureGameInProgress(string error = "Game is not in progress")
    {
        if (_gameState.GameStatus != GameStatuses.InProgress)
        {
            throw new InvalidOperationException(error);
        }
    }

    private void EnsureHost(string sid, string error = "Not the host")
    {
        var player = GetRequiredPlayer(sid);

        if (!player.IsHost)
        {
            throw new UnauthorizedAccessException(error);
        }
    }

    private void EnsureResultsPhase(string error = "Game is not in results phase")
    {
        EnsureGameInProgress();

        if (_gameState.GamePhase != GamePhases.Results)
        {
            throw new InvalidOperationException(error);
        }
    }

    private void EnsureVotingPhase(string error = "Game is not in voting phase")
    {
        EnsureGameInProgress();

        if (_gameState.GamePhase != GamePhases.Voting)
        {
            throw new InvalidOperationException(error);
        }
    }

    private PlayerState GetRequiredPlayer(string sid)
    {
        var player = _gameState.Players.FirstOrDefault(p => p.Sid == sid);

        if (player is null)
        {
            throw new InvalidOperationException("Invalid session id");
        }

        return player;
    }

    private PlayerState GetRequiredPlayer(int playerId)
    {
        var player = _gameState.Players.FirstOrDefault(p => p.PlayerId == playerId);

        if (player is null)
        {
            throw new InvalidOperationException("Player not found");
        }

        return player;
    }

    private ISessionActor GetSessionActor(string sid) =>
        ProxyFactory.CreateActorProxy<ISessionActor>(
            new ActorId(sid),
            typeof(SessionActor).Name);

    private Task SaveGameState(CancellationToken cancellationToken = default) =>
        StateManager.SetStateAsync(GameStateName, _gameState, cancellationToken);

    private async Task SetInactivityReminder(CancellationToken cancellationToken = default)
    {
        // Remove previous reminder if one exists
        await UnregisterReminderAsync(InactivityReminderName);

        await RegisterReminderAsync(
            InactivityReminderName,
            Array.Empty<byte>(),
            TimeSpan.FromDays(1),
            TimeSpan.FromMilliseconds(-1));
    }
}
