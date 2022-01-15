namespace Scrummy.GameService.Api.Actors;

public interface ILobbyActor : IActor
{
    Task<string> CreateGame(CancellationToken cancellationToken = default);
    Task<bool> GameExists(string gameId, CancellationToken cancellationToken = default);
    Task NotifyGameEnded(string gameId, CancellationToken cancellationToken = default);
}
