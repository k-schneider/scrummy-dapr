namespace Scrummy.GameService.Api.Actors;

public class GameActor : Actor, IGameActor
{
    private readonly DaprClient _dapr;

    private GameStatus _gameStatus = GameStatus.None;
    private int _playerCounter = 0;
    private List<PlayerState> _players = new();

    private string GameId => Id.GetId();

    public GameActor(ActorHost host, DaprClient dapr)
        : base(host)
    {
        _dapr = dapr;
    }

    public async Task StartGame(CancellationToken cancellationToken = default)
    {
        if (_gameStatus != GameStatus.None)
        {
            throw new InvalidOperationException("Game has already started");
        }

        _gameStatus = GameStatus.InProgress;

        await _dapr.PublishEventAsync(
            Constants.DaprPubSubName,
            typeof(GameStartedEvent).Name,
            new GameStartedEvent(GameId),
            cancellationToken);
    }

    public async Task<(string sid, int playerId)> AddPlayer(string nickname, CancellationToken cancellationToken = default)
    {
        var sid = NewSid();
        var playerId = NextPlayerId();

        await GetSessionActor(sid).SetGameId(GameId, cancellationToken);

        _players.Add(new PlayerState
        {
            Sid = sid,
            PlayerId = playerId,
            Nickname = nickname
        });

        await _dapr.PublishEventAsync(
            Constants.DaprPubSubName,
            typeof(PlayerJoinedGameEvent).Name,
            new PlayerJoinedGameEvent(sid, playerId, nickname),
            cancellationToken);

        return (sid, playerId);
    }

    public async Task RemovePlayer(string sid, CancellationToken cancellationToken = default)
    {
        var player = _players.Where(p => p.Sid == sid).FirstOrDefault();

        if (player == null)
        {
            throw new InvalidOperationException("Player not found");
        }

        _players.Remove(player);

        await _dapr.PublishEventAsync(
            Constants.DaprPubSubName,
            typeof(PlayerLeftGameEvent).Name,
            new PlayerLeftGameEvent(player.Sid, player.PlayerId, player.Nickname),
            cancellationToken);

        // todo: considerations...
        //   clear gameId SessionActor?
        //   remove from hub group?
        //   terminate connection somehow?
        //   if no more players left, end game?
    }

    private string NewSid() => Guid.NewGuid().ToString();
    private int NextPlayerId() => _playerCounter++;

    private ISessionActor GetSessionActor(string sid) =>
        ProxyFactory.CreateActorProxy<ISessionActor>(
            new ActorId(sid),
            typeof(SessionActor).Name);
}
