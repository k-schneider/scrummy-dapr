namespace Scrummy.GameService.Api.Actors;

public interface IGameActor : IActor
{
    Task<(string sid, int playerId)> AddPlayer(string nickname, CancellationToken cancellationToken = default);
    Task<GameSnapshot> GetGameSnapshot(CancellationToken cancellationToken = default);
    Task RemovePlayer(string sid, CancellationToken cancellationToken = default);
    Task StartGame(CancellationToken cancellationToken = default);
}
