namespace Scrummy.GameService.Api.Actors;

public class GameActor : Actor, IGameActor
{
    private readonly IEventBus _eventBus;

    private GameStatus _gameStatus = GameStatus.None;
    private GamePhase _gamePhase = GamePhase.Voting;
    private int _playerCounter;
    private List<PlayerState> _players = new();
    private Dictionary<int, string> _votes = new();
    private HashSet<string> _deck = new()
    {
        "0",
        "1",
        "2",
        "3",
        "5",
        "8",
        "13",
        "20",
        "40",
        "100",
        "?"
    };

    private string GameId => Id.GetId();

    public GameActor(ActorHost host, IEventBus eventBus)
        : base(host)
    {
        _eventBus = eventBus;
    }

    public async Task<(string sid, int playerId)> AddPlayer(string nickname, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        var sid = NewSid();
        var playerId = NextPlayerId();

        await GetSessionActor(sid).AssociateWithGame(GameId, playerId, cancellationToken);

        _players.Add(new PlayerState
        {
            Sid = sid,
            PlayerId = playerId,
            Nickname = nickname,
            IsHost = !_players.Any()
        });

        await _eventBus.PublishAsync(
            new PlayerJoinedGameIntegrationEvent(
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

        if (!_deck.Contains(vote))
        {
            throw new InvalidOperationException("Invalid vote");
        }

        var player = GetRequiredPlayer(sid);

        _votes.TryGetValue(player.PlayerId, out var previousVote);
        _votes[player.PlayerId] = vote;

        await _eventBus.PublishAsync(
            new PlayerVoteCastIntegrationEvent(
                player.Sid,
                player.PlayerId,
                vote,
                previousVote,
                GameId),
            cancellationToken);
    }

    public async Task FlipCards(string sid, CancellationToken cancellationToken = default)
    {
        EnsureVotingPhase();
        EnsureHost(sid, "Only host can flip cards");

        _gamePhase = GamePhase.Results;

        await _eventBus.PublishAsync(
            new CardsFlippedIntegrationEvent(GameId, _votes),
            cancellationToken);
    }

    public Task<GameSnapshot> GetGameSnapshot(int playerId, CancellationToken cancellationToken = default)
    {
        var players = _players.Select(p => new PlayerSnapshot(
            p.PlayerId,
            p.Nickname,
            p.IsHost,
            p.IsConnected)).ToList();

        var votes = _votes.ToDictionary(kvp =>
            kvp.Key,
            // only return vote values for other players when showing results
            kvp => kvp.Key == playerId || _gamePhase == GamePhase.Results ? kvp.Value : null);

        return Task.FromResult(new GameSnapshot(
            GameId,
            _gamePhase.Name,
            players,
            _deck,
            votes));
    }

    public Task NotifyPlayerConnected(int playerId, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        _players
            .First(p => p.PlayerId == playerId)
            .IsConnected = true;

        return Task.CompletedTask;
    }

    public Task NotifyPlayerDisconnected(int playerId, CancellationToken cancellationToken = default)
    {
        var player = _players
            .FirstOrDefault(p => p.PlayerId == playerId);

        if (player is not null)
        {
            player.IsConnected = false;
        }

        return Task.CompletedTask;
    }

    public async Task PlayAgain(string sid, CancellationToken cancellationToken = default)
    {
        EnsureResultsPhase();
        EnsureHost(sid, "Only host can choose to play again");

        _votes.Clear();
        _gamePhase = GamePhase.Voting;

        await _eventBus.PublishAsync(
            new NewVoteStartedIntegrationEvent(GameId),
            cancellationToken);
    }

    public async Task RecallVote(string sid, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        var player = GetRequiredPlayer(sid);

        _votes.TryGetValue(player.PlayerId, out var previousVote);
        _votes.Remove(player.PlayerId);

        await _eventBus.PublishAsync(
            new PlayerVoteRecalledIntegrationEvent(
                player.Sid,
                player.PlayerId,
                previousVote,
                GameId),
            cancellationToken);
    }

    public async Task RemovePlayer(string sid, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();
        var player = GetRequiredPlayer(sid);

        _players.Remove(player);
        _votes.Remove(player.PlayerId);

        await _eventBus.PublishAsync(
            new PlayerLeftGameIntegrationEvent(
                player.Sid,
                player.PlayerId,
                GameId),
            cancellationToken);

        // If the player was the host, pick a new host based on when they joined
        if (player.IsHost && _players.Any())
        {
            var newHost = _players.OrderBy(p => p.JoinDate).First();
            newHost.IsHost = true;

            await _eventBus.PublishAsync(
                new HostChangedIntegrationEvent(
                    player.Sid,
                    player.PlayerId,
                    player.Nickname,
                    newHost.Sid,
                    newHost.PlayerId,
                    newHost.Nickname,
                    GameId),
                cancellationToken);
        }

        if (!_players.Any())
        {
            _gameStatus = GameStatus.GameOver;

            await _eventBus.PublishAsync(
                new GameEndedIntegrationEvent(
                    GameId),
                cancellationToken);
        }

        // todo: considerations...
        //   clear gameId SessionActor?
        //   remove from hub group?
        //   terminate connection somehow?
    }

    public async Task ResetVotes(string sid, CancellationToken cancellationToken = default)
    {
        EnsureVotingPhase();
        EnsureHost(sid, "Only host can reset votes");

        _votes.Clear();

        await _eventBus.PublishAsync(
            new VotesResetIntegrationEvent(GameId),
            cancellationToken);
    }

    public async Task StartGame(CancellationToken cancellationToken = default)
    {
        if (_gameStatus == GameStatus.InProgress)
        {
            throw new InvalidOperationException("Game has already started");
        }

        _gameStatus = GameStatus.InProgress;
        _gamePhase = GamePhase.Voting;
        _playerCounter = 0;
        _players = new();
        _votes = new();

        await _eventBus.PublishAsync(
            new GameStartedIntegrationEvent(GameId),
            cancellationToken);
    }

    public async Task UpdateNickname(string sid, string nickname, CancellationToken cancellationToken = default)
    {
        EnsureGameInProgress();

        var player = GetRequiredPlayer(sid);

        player.Nickname = nickname;

        await _eventBus.PublishAsync(
            new PlayerNicknameChangedIntegrationEvent(
                player.Sid,
                player.PlayerId,
                nickname,
                GameId),
            cancellationToken);
    }

    private string NewSid() => Guid.NewGuid().ToString();
    private int NextPlayerId() => ++_playerCounter;

    private void EnsureGameInProgress(string error = "Game is not in progress")
    {
        if (_gameStatus != GameStatus.InProgress)
        {
            throw new InvalidOperationException(error);
        }
    }

    private void EnsureHost(string sid, string error = "Not the host")
    {
        var player = _players.First(p => p.Sid == sid);

        if (!player.IsHost)
        {
            throw new InvalidOperationException(error);
        }
    }

    private void EnsureResultsPhase(string error = "Game is not in results phase")
    {
        EnsureGameInProgress();

        if (_gamePhase != GamePhase.Results)
        {
            throw new InvalidOperationException(error);
        }
    }

    private void EnsureVotingPhase(string error = "Game is not in voting phase")
    {
        EnsureGameInProgress();

        if (_gamePhase != GamePhase.Voting)
        {
            throw new InvalidOperationException(error);
        }
    }

    private PlayerState GetRequiredPlayer(string sid)
    {
        var player = _players.FirstOrDefault(p => p.Sid == sid);

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
}
