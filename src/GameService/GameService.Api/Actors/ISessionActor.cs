namespace Scrummy.GameService.Api.Actors;

public interface ISessionActor : IActor
{
    Task AddConnection(string connectionId, CancellationToken cancellationToken = default);
    Task AssociateWithGame(string gameId, int playerId, CancellationToken cancellationToken = default);
    Task<string?> GetGameId(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetConnectionIds(CancellationToken cancellationToken = default);
    Task RemoveConnection(string connectionId, CancellationToken cancellationToken = default);
    Task Reset(CancellationToken cancellationToken = default);
}
