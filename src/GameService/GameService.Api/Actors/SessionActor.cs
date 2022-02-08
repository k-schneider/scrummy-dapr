namespace Scrummy.GameService.Api.Actors;

public class SessionActor : Actor, ISessionActor
{
    private const string SessionStateName = "SessionState";

    private readonly IEventBus _eventBus;
    private SessionState _sessionState = null!;

    private string Sid => Id.GetId();

    public SessionActor(ActorHost host, IEventBus eventBus)
        : base(host)
    {
        _eventBus = eventBus;
    }

    protected override async Task OnActivateAsync()
    {
        var sessionState = await StateManager.TryGetStateAsync<SessionState>(SessionStateName);
        _sessionState = sessionState.HasValue ? sessionState.Value : new SessionState();

        await base.OnActivateAsync();
    }

    public async Task AddConnection(string connectionId, CancellationToken cancellationToken = default)
    {
        if (_sessionState.GameId is null)
        {
            throw new InvalidOperationException("Session is not associated with a game");
        }

        if (_sessionState.ConnectionIds.Add(connectionId))
        {
            await SaveSessionState();

            await _eventBus.PublishAsync(
                new PlayerConnectedIntegrationEvent(
                    connectionId,
                    Sid,
                    _sessionState.GameId,
                    _sessionState.PlayerId,
                    _sessionState.ConnectionIds.Count),
                cancellationToken);
        }
    }

    public async Task AssociateWithGame(string gameId, int playerId, CancellationToken cancellationToken = default)
    {
        if (_sessionState.GameId is not null && _sessionState.GameId != gameId)
        {
            throw new InvalidOperationException("Session is already associated with a different game");
        }

        _sessionState.GameId = gameId;
        _sessionState.PlayerId = playerId;

        await SaveSessionState();
    }

    public Task<IEnumerable<string>> GetConnectionIds(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_sessionState.ConnectionIds.AsEnumerable());
    }

    public Task<string?> GetGameId(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_sessionState.GameId);
    }

    public async Task RemoveConnection(string connectionId, CancellationToken cancellationToken = default)
    {
        if (_sessionState.ConnectionIds.Remove(connectionId))
        {
            await SaveSessionState();

            await _eventBus.PublishAsync(
                new PlayerDisconnectedIntegrationEvent(
                    connectionId,
                    Sid,
                    _sessionState.GameId!,
                    _sessionState.PlayerId,
                    _sessionState.ConnectionIds.Count),
                cancellationToken);
        }
    }

    private Task SaveSessionState() =>
        StateManager.SetStateAsync(SessionStateName, _sessionState);
}
