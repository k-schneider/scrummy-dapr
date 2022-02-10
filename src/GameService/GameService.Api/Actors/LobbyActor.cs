namespace Scrummy.GameService.Api.Actors;

public class LobbyActor : Actor, ILobbyActor
{
    private const string LobbyStateName = "LobbyState";

    private readonly DaprClient _dapr;
    private LobbyState _lobbyState = null!;

    public LobbyActor(ActorHost host, DaprClient dapr)
        : base(host)
    {
        _dapr = dapr;
    }

    protected override async Task OnActivateAsync()
    {
        var lobbyState = await StateManager.TryGetStateAsync<LobbyState>(LobbyStateName);
        _lobbyState = lobbyState.HasValue ? lobbyState.Value : new LobbyState();

        await base.OnActivateAsync();
    }

    public async Task<string> CreateGame(IEnumerable<Card> deck, CancellationToken cancellationToken = default)
    {
        string gameId = GenerateGameId();

        await GetGameActor(gameId).StartGame(deck, cancellationToken);

        _lobbyState.Games.Add(gameId);

        await SaveLobbyState(cancellationToken);

        return gameId;
    }

    public Task<bool> GameExists(string gameId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_lobbyState.Games.Contains(gameId));
    }

    public async Task NotifyGameEnded(string gameId, CancellationToken cancellationToken = default)
    {
        _lobbyState.Games.Remove(gameId);

        await SaveLobbyState(cancellationToken);
    }

    private string GenerateGameId()
    {
        string? result = null;

        while (result is null)
        {
            var gameId = GameId.Generate();

            if (_lobbyState.Games.Contains(gameId))
            {
                continue;
            }

            result = gameId;
        }

        return result;
    }

    private IGameActor GetGameActor(string gameId) =>
        ProxyFactory.CreateActorProxy<IGameActor>(
            new ActorId(gameId),
            typeof(GameActor).Name);

    private Task SaveLobbyState(CancellationToken cancellationToken = default) =>
        StateManager.SetStateAsync(LobbyStateName, _lobbyState, cancellationToken);
}
