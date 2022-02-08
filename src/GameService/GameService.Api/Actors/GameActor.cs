namespace Scrummy.GameService.Api.Actors;

public class GameActor : Actor, IGameActor
{
    private const string GameStateName = "GameState";

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

        await SaveGameState();

        await _eventBus.PublishAsync(
            new PlayerJoinedIntegrationEvent(
                sid,
                playerId,
                nickname,
                GameId),
            cancellationToken);

        return (sid, playerId);
    }

    public async Task CastVote(string sid, string vote, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        if (!_gameState.Deck.Contains(vote))
        {
            throw new InvalidOperationException("Invalid vote");
        }

        var self = GetRequiredPlayer(sid);

        _gameState.Votes.TryGetValue(self.PlayerId, out var previousVote);
        _gameState.Votes[self.PlayerId] = vote;

        await SaveGameState();

        await _eventBus.PublishAsync(
            new PlayerVoteCastIntegrationEvent(
                self.Sid,
                self.PlayerId,
                vote,
                previousVote,
                GameId),
            cancellationToken);
    }

    public async Task FlipCards(string sid, CancellationToken cancellationToken = default)
    {
        EnsureVotingPhase();
        EnsureHost(sid, "Only host can flip cards");

        _gameState.GamePhase = GamePhases.Results;

        await SaveGameState();

        await _eventBus.PublishAsync(
            new CardsFlippedIntegrationEvent(GameId, _gameState.Votes),
            cancellationToken);
    }

    public Task<Game> GetGameState(int playerId, CancellationToken cancellationToken = default)
    {
        var players = _gameState.Players.Select(p => new Player(
            p.PlayerId,
            p.Nickname,
            p.IsHost,
            p.IsConnected)).ToList();

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

        await SaveGameState();

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

            await SaveGameState();

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

            await SaveGameState();

            await _eventBus.PublishAsync(
                new GameEndedIntegrationEvent(
                    GameId),
                cancellationToken);
        }

        // todo: considerations...
        //   clear gameId SessionActor?
        //   remove from hub group?
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
    }

    public async Task NotifyPlayerConnected(int playerId, CancellationToken cancellationToken = default)
    {
        var player = _gameState.Players
            .FirstOrDefault(p => p.PlayerId == playerId);

        if (player is not null)
        {
            player.IsConnected = true;
            await SaveGameState();
        }
    }

    public async Task NotifyPlayerDisconnected(int playerId, CancellationToken cancellationToken = default)
    {
        var player = _gameState.Players
            .FirstOrDefault(p => p.PlayerId == playerId);

        if (player is not null)
        {
            player.IsConnected = false;
            await SaveGameState();
        }
    }

    public async Task PlayAgain(string sid, CancellationToken cancellationToken = default)
    {
        EnsureResultsPhase();
        EnsureHost(sid, "Only host can choose to play again");

        _gameState.Votes.Clear();
        _gameState.GamePhase = GamePhases.Voting;

        await SaveGameState();

        await _eventBus.PublishAsync(
            new NewVoteStartedIntegrationEvent(GameId),
            cancellationToken);
    }

    public async Task PromotePlayer(string sid, int playerId, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();
        EnsureHost(sid, "Only host can promote other players");

        var self = GetRequiredPlayer(sid);
        var newHost = GetRequiredPlayer(playerId);

        self.IsHost = false;
        newHost.IsHost = true;

        await SaveGameState();

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

    public async Task RecallVote(string sid, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        var player = GetRequiredPlayer(sid);

        _gameState.Votes.TryGetValue(player.PlayerId, out var previousVote);
        _gameState.Votes.Remove(player.PlayerId);

        await SaveGameState();

        await _eventBus.PublishAsync(
            new PlayerVoteRecalledIntegrationEvent(
                player.Sid,
                player.PlayerId,
                previousVote,
                GameId),
            cancellationToken);
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

        await SaveGameState();

        await _eventBus.PublishAsync(
            new PlayerRemovedIntegrationEvent(
                player.Sid,
                player.PlayerId,
                GameId),
            cancellationToken);
    }

    public async Task ResetVotes(string sid, CancellationToken cancellationToken = default)
    {
        EnsureVotingPhase();
        EnsureHost(sid, "Only host can reset votes");

        _gameState.Votes.Clear();

        await SaveGameState();

        await _eventBus.PublishAsync(
            new VotesResetIntegrationEvent(GameId),
            cancellationToken);
    }

    public async Task StartGame(CancellationToken cancellationToken = default)
    {
        if (_gameState.GameStatus == GameStatuses.InProgress)
        {
            throw new InvalidOperationException("Game has already started");
        }

        _gameState.GameStatus = GameStatuses.InProgress;
        _gameState.GamePhase = GamePhases.Voting;
        _gameState.PlayerCounter = 0;
        _gameState.Players.Clear();
        _gameState.Votes.Clear();

        await SaveGameState();

        await _eventBus.PublishAsync(
            new GameStartedIntegrationEvent(GameId),
            cancellationToken);
    }

    public async Task UpdateNickname(string sid, string nickname, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        var self = GetRequiredPlayer(sid);

        self.Nickname = nickname;

        await _eventBus.PublishAsync(
            new PlayerNicknameChangedIntegrationEvent(
                self.Sid,
                self.PlayerId,
                nickname,
                GameId),
            cancellationToken);
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

        if (_gameState.GameStatus != GamePhases.Results)
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

    private Task SaveGameState() =>
        StateManager.SetStateAsync(GameStateName, _gameState);
}
