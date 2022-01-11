namespace Scrummy.GameService.Api.Actors;

public class PlayerActor : Actor, IPlayerActor
{
    private readonly DaprClient _dapr;
    private readonly Dictionary<string, string> _gameConnections = new();

    private string PlayerId => Id.GetId();

    public PlayerActor(ActorHost host, DaprClient dapr)
        : base(host)
    {
        _dapr = dapr;
    }

    public async Task HandleDisconnect(string connectionId, CancellationToken cancellationToken = default)
    {
        if (_gameConnections.TryGetValue(connectionId, out var gameId))
        {
            _gameConnections.Remove(connectionId);
        }

        // If a game was found for the connection, and there are no more connections for that game then publish an event
        if (!string.IsNullOrEmpty(gameId) && !_gameConnections.Where(x => x.Value == gameId).Any())
        {
            await _dapr.PublishEventAsync(
                Constants.DaprPubSubName,
                typeof(PlayerDisconnectedFromGameEvent).Name,
                new PlayerDisconnectedFromGameEvent(PlayerId, gameId),
                cancellationToken);
        }
    }

    public async Task JoinGame(string gameId, string nickname, string connectionId, CancellationToken cancellationToken = default)
    {
        _gameConnections[connectionId] = gameId;

        await _dapr.PublishEventAsync(
            Constants.DaprPubSubName,
            typeof(PlayerJoinedGameEvent).Name,
            new PlayerJoinedGameEvent(PlayerId, gameId, nickname),
            cancellationToken);
    }

    public async Task LeaveGame(string gameId, CancellationToken cancellationToken = default)
    {
        var connectionIds = _gameConnections.Where(x => x.Value == gameId).Select(x => x.Key).ToList();

        foreach (var connectionId in connectionIds)
        {
            _gameConnections.Remove(connectionId);
        }

        await _dapr.PublishEventAsync(
            Constants.DaprPubSubName,
            typeof(PlayerLeftGameEvent).Name,
            new PlayerLeftGameEvent(PlayerId, gameId),
            cancellationToken);
    }
}
