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

        return (sid, playerId);
    }

    public Task RemovePlayer(string sid, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private string NewSid() => Guid.NewGuid().ToString();
    private int NextPlayerId() => _playerCounter++;

    private ISessionActor GetSessionActor(string sid)
    {
        return ProxyFactory.CreateActorProxy<ISessionActor>(
            new ActorId(sid),
            typeof(SessionActor).Name);
    }
}
