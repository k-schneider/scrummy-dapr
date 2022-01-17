namespace Scrummy.GameService.Api.Actors;

public class SessionActor : Actor, ISessionActor
{
    private readonly IEventBus _eventBus;
    private HashSet<string> _connectionIds = new HashSet<string>();
    private string? _gameId { get; set; }
    private int _playerId { get; set; }

    private string Sid => Id.GetId();

    public SessionActor(ActorHost host, IEventBus eventBus)
        : base(host)
    {
        _eventBus = eventBus;
    }

    public async Task AddConnection(string connectionId, CancellationToken cancellationToken = default)
    {
        if (_gameId is null)
        {
            throw new InvalidOperationException("Session is not associated with a game");
        }

        if (_connectionIds.Add(connectionId))
        {
            await _eventBus.PublishAsync(
                new PlayerConnectedIntegrationEvent(
                    connectionId,
                    Sid,
                    _gameId,
                    _playerId,
                    _connectionIds.Count),
                cancellationToken);
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
            await _eventBus.PublishAsync(
                new PlayerDisconnectedIntegrationEvent(
                    connectionId,
                    Sid,
                    _gameId!,
                    _playerId,
                    _connectionIds.Count),
                cancellationToken);
        }
    }

    public Task AssociateWithGame(string gameId, int playerId, CancellationToken cancellationToken = default)
    {
        if (_gameId is not null && _gameId != gameId)
        {
            throw new InvalidOperationException("Session is already associated with a different game");
        }

        _gameId = gameId;
        _playerId = playerId;
        return Task.CompletedTask;
    }
}
