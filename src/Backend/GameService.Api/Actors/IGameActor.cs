namespace Scrummy.GameService.Api.Actors;

public interface IGameActor : IActor
{
    Task NotifyPlayerDisconnected(string playerId, CancellationToken cancellationToken = default);
    Task NotifyPlayerJoined(string playerId, string nickname, CancellationToken cancellationToken = default);
    Task NotifyPlayerLeft(string playerId, CancellationToken cancellationToken = default);
    Task Start(CancellationToken cancellationToken = default);
}
