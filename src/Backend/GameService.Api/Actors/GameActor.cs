namespace Scrummy.GameService.Api.Actors;

public class GameActor : Actor, IGameActor
{
    private readonly DaprClient _dapr;

    private GameStatus _gameStatus = GameStatus.None;
    private Dictionary<string, PlayerState> _players = new();

    private string GameId => Id.GetId();

    public GameActor(ActorHost host, DaprClient dapr)
        : base(host)
    {
        _dapr = dapr;
    }

    public Task NotifyPlayerDisconnected(string playerId, CancellationToken cancellationToken = default)
    {
        _players[playerId].IsConnected = false;
        return Task.CompletedTask;
    }

    public Task NotifyPlayerJoined(string playerId, string nickname, CancellationToken cancellationToken = default)
    {
        _players[playerId] = new PlayerState
        {
            Nickname = nickname,
            IsConnected = true
        };
        return Task.CompletedTask;
    }

    public Task NotifyPlayerLeft(string playerId, CancellationToken cancellationToken = default)
    {
        _players.Remove(playerId);
        return Task.CompletedTask;
    }

    public Task Start(CancellationToken cancellationToken = default)
    {
        if (_gameStatus != GameStatus.None)
        {
            throw new InvalidOperationException("Game has already started");
        }

        _gameStatus = GameStatus.InProgress;
        return Task.CompletedTask;
    }
}
