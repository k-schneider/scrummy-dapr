public interface IGameActor : IActor
{
    Task<bool> Exists(CancellationToken cancellationToken = default);
    Task<GameState> GetGameState(CancellationToken cancellationToken = default);
    Task Start(CancellationToken cancellationToken = default);
}
