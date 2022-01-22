﻿namespace Scrummy.WebBlazor.Client;

public interface IAppApi
{
    [Post("g/game/{gameId}/vote")]
    Task CastVote([Path] string gameId, [Body] CastVoteRequest request, CancellationToken cancellationToken = default);

    [Post("g/lobby/create")]
    Task<CreateGameResponse> CreateGame([Body] CreateGameRequest request, CancellationToken cancellationToken = default);

    [Post("g/game/{gameId}/flip")]
    Task FlipCards([Path] string gameId, [Body] FlipCardsRequest request, CancellationToken cancellationToken = default);

    [Post("g/game/{gameId}/join")]
    Task<JoinGameResponse> JoinGame([Path] string gameId, [Body] JoinGameRequest request, CancellationToken cancellationToken = default);

    [Post("g/game/{gameId}/leave")]
    Task LeaveGame([Path] string gameId, [Body] LeaveGameRequest request, CancellationToken cancellationToken = default);

    [Post("g/game/{gameId}/next")]
    Task PlayAgain([Path] string gameId, [Body] PlayAgainRequest request, CancellationToken cancellationToken = default);

    [Delete("g/game/{gameId}/vote")]
    Task RecallVote([Path] string gameId, [Body] RecallVoteRequest request, CancellationToken cancellationToken = default);

    [Delete("g/game/{gameId}/votes")]
    Task ResetVotes([Path] string gameId, [Body] ResetVotesRequest request, CancellationToken cancellationToken = default);
}
