namespace Scrummy.GameService.Api.Actors;

public class SessionActor : Actor, ISessionActor
{
    private readonly DaprClient _dapr;
    private HashSet<string> _connectionIds = new HashSet<string>();
    private string? _gameId { get; set; }

    private string Sid => Id.GetId();

    public SessionActor(ActorHost host, DaprClient dapr)
        : base(host)
    {
        _dapr = dapr;
    }

    public async Task AddConnection(string connectionId, CancellationToken cancellationToken = default)
    {
        if (_gameId is null)
        {
            throw new InvalidOperationException("Session is not associated with a game");
        }

        if (_connectionIds.Add(connectionId))
        {
            await _dapr.PublishEventAsync(
                Constants.DaprPubSubName,
                typeof(SessionConnectedEvent).Name,
                new SessionConnectedEvent(connectionId, Sid, _gameId),
                cancellationToken);

            if (_connectionIds.Count == 1)
            {
                // todo: publish player online event so that game can update player state
            }
        }
    }

    public Task<string?> GetGameId(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_gameId);
    }

    public async Task RemoveConnection(string connectionId, CancellationToken cancellationToken = default)
    {
        if (_connectionIds.Remove(connectionId))
        {
            await _dapr.PublishEventAsync(
                Constants.DaprPubSubName,
                typeof(SessionDisconnectedEvent).Name,
                new SessionDisconnectedEvent(connectionId, Sid, _gameId!),
                cancellationToken);

            if (_connectionIds.Count == 0)
            {
                // todo: publish player offline event so that game can update player state
            }
        }
    }

    public Task SetGameId(string gameId, CancellationToken cancellationToken = default)
    {
        if (_gameId is not null && _gameId != gameId)
        {
            throw new InvalidOperationException("Session is already associated with a different game");
        }

        _gameId = gameId;
        return Task.CompletedTask;
    }
}
