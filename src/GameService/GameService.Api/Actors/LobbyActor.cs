namespace Scrummy.GameService.Api.Actors;

public class LobbyActor : Actor, ILobbyActor
{
    private readonly DaprClient _dapr;
    private readonly HashSet<string> _games = new();

    public LobbyActor(ActorHost host, DaprClient dapr)
        : base(host)
    {
        _dapr = dapr;
    }

    public async Task<string> CreateGame(CancellationToken cancellationToken = default)
    {
        string gameId = GenerateGameId();

        await GetGameActor(gameId).StartGame(cancellationToken);

        _games.Add(gameId);

        return gameId;
    }

    public Task<bool> GameExists(string gameId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_games.Contains(gameId));
    }

    public Task NotifyGameEnded(string gameId, CancellationToken cancellationToken = default)
    {
        _games.Remove(gameId);
        return Task.CompletedTask;
    }

    private string GenerateGameId()
    {
        string? result = null;

        while (result is null)
        {
            var gameId = GameId.Generate();

            if (_games.Contains(gameId))
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
}
