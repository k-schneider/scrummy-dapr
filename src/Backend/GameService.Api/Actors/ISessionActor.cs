namespace Scrummy.GameService.Api.Actors;

public interface ISessionActor : IActor
{
    Task AddConnection(string connectionId, CancellationToken cancellationToken = default);
    Task RemoveConnection(string connectionId, CancellationToken cancellationToken = default);
    Task SetGameId(string gameId, CancellationToken cancellationToken = default);
    Task<string?> GetGameId(CancellationToken cancellationToken = default);
}
