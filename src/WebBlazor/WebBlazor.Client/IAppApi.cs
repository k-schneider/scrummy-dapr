namespace Scrummy.WebBlazor.Client;

public interface IAppApi
{
    [Post("g/lobby/create")]
    Task<GameSession> CreateGame([Body] CreateGameRequest request, CancellationToken cancellationToken = default);

    [Post("g/game/{gameId}/join")]
    Task<GameSession> JoinGame([Path] string gameId, [Body] JoinGameRequest request, CancellationToken cancellationToken = default);

    [Post("g/game/{gameId}/leave")]
    Task LeaveGame([Path] string gameId, [Body] LeaveGameRequest request, CancellationToken cancellationToken = default);
}
