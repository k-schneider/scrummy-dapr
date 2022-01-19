namespace Scrummy.GameService.Api.Actors;

public class GameActor : Actor, IGameActor
{
    private readonly IEventBus _eventBus;

    private GameStatus _gameStatus = GameStatus.None;
    private int _playerCounter;
    private List<PlayerState> _players = new();
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
        if (!_deck.Contains(vote))
        {
            throw new InvalidOperationException("Invalid vote");
        }

        var player = _players.First(p => p.Sid == sid);
        player.Vote = vote;

        await _eventBus.PublishAsync(
            new PlayerVotedIntegrationEvent(
                player.Sid,
                player.PlayerId,
                vote,
                GameId),
            cancellationToken);
    }

    public Task<GameSnapshot> GetGameSnapshot(int playerId, CancellationToken cancellationToken = default)
    {
        var players = _players.Select(p => new PlayerSnapshot(
            p.PlayerId,
            p.Nickname,
            p.IsHost,
            p.IsConnected,
            p.Vote is not null)).ToList();

        return Task.FromResult(new GameSnapshot(
            GameId,
            players,
            _deck,
            _players.First(p => p.PlayerId == playerId).Vote));
    }

    public Task NotifyPlayerConnected(int playerId, CancellationToken cancellationToken = default)
    {
        _players
            .First(p => p.PlayerId == playerId)
            .IsConnected = true;

        return Task.CompletedTask;
    }

    public async Task RemovePlayer(string sid, CancellationToken cancellationToken = default)
    {
        var player = _players.Where(p => p.Sid == sid).FirstOrDefault();

        if (player == null)
        {
            throw new InvalidOperationException("Player not found");
        }

        _players.Remove(player);

        await _eventBus.PublishAsync(
            new PlayerLeftGameIntegrationEvent(
                player.Sid,
                player.PlayerId,
                GameId),
            cancellationToken);

        // todo: if player that left was host, reassign host to olded JoinDate player and send integration event

        // todo: considerations...
        //   clear gameId SessionActor?
        //   remove from hub group?
        //   terminate connection somehow?
        //   if no more players left, end game?
    }

    public async Task StartGame(CancellationToken cancellationToken = default)
    {
        if (_gameStatus != GameStatus.None)
        {
            throw new InvalidOperationException("Game has already started");
        }

        _gameStatus = GameStatus.InProgress;

        await _eventBus.PublishAsync(
            new GameStartedIntegrationEvent(GameId),
            cancellationToken);
    }

    private string NewSid() => Guid.NewGuid().ToString();
    private int NextPlayerId() => ++_playerCounter;

    private ISessionActor GetSessionActor(string sid) =>
        ProxyFactory.CreateActorProxy<ISessionActor>(
            new ActorId(sid),
            typeof(SessionActor).Name);
}
