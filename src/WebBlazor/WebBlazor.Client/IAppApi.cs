namespace Scrummy.WebBlazor.Client;

public interface IAppApi
{
    [Post("g/game/{gameId}/vote")]
    Task<CreateGameResponse> CastVote([Path] string gameId, [Body] CastVoteRequest request, CancellationToken cancellationToken = default);

    [Post("g/lobby/create")]
    Task<CreateGameResponse> CreateGame([Body] CreateGameRequest request, CancellationToken cancellationToken = default);

    [Post("g/game/{gameId}/join")]
    Task<JoinGameResponse> JoinGame([Path] string gameId, [Body] JoinGameRequest request, CancellationToken cancellationToken = default);

    [Post("g/game/{gameId}/leave")]
    Task LeaveGame([Path] string gameId, [Body] LeaveGameRequest request, CancellationToken cancellationToken = default);
}
