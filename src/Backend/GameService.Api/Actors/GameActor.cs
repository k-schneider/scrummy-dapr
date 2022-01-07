namespace Scrummy.GameService.Api.Actors;

public class GameActor : Actor, IGameActor
{
    private const string GameStateName = "Game";

    private string GameId => Id.GetId();

    public GameActor(ActorHost host)
        : base(host)
    {
    }

    public async Task<bool> Exists(CancellationToken cancellationToken = default)
    {
        var result = await StateManager.TryGetStateAsync<GameState>(GameStateName, cancellationToken);
        return result.HasValue;
    }

    public Task<GameState> GetGameState(CancellationToken cancellationToken = default)
    {
        return StateManager.GetStateAsync<GameState>(GameStateName, cancellationToken);
    }

    public async Task Start(CancellationToken cancellationToken = default)
    {
        if (await Exists())
        {
            throw new InvalidOperationException("Game is already in progress.");
        }

        await StateManager.SetStateAsync<GameState>(GameStateName, new GameState(), cancellationToken);
    }
}
