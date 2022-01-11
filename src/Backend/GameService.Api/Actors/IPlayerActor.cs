namespace Scrummy.GameService.Api.Actors;

public interface IPlayerActor : IActor
{
    Task JoinGame(string gameId, string nickname, string connectionId, CancellationToken cancellationToken = default);
    Task LeaveGame(string gameId, CancellationToken cancellationToken = default);
    Task HandleDisconnect(string connectionId, CancellationToken cancellationToken = default);
}
