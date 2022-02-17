namespace Scrummy.WebBlazor.Client;

public interface IAppApi
{
    [Post("g/player/{sid}/vote")]
    Task CastVote([Path] string sid, [Body] CastVoteRequest request, CancellationToken cancellationToken = default);

    [Post("g/lobby/create")]
    Task<CreateGameResponse> CreateGame([Body] CreateGameRequest request, CancellationToken cancellationToken = default);

    [Post("g/player/{sid}/flip")]
    Task FlipCards([Path] string sid, CancellationToken cancellationToken = default);

    [Get("g/game/{gameId}")]
    Task<bool> GameExists([Path] string gameId, CancellationToken cancellationToken = default);

    [Post("g/game/{gameId}/join")]
    Task<JoinGameResponse> JoinGame([Path] string gameId, [Body] JoinGameRequest request, CancellationToken cancellationToken = default);

    [Post("g/player/{sid}/leave")]
    Task LeaveGame([Path] string sid, CancellationToken cancellationToken = default);

    [Post("g/player/{sid}/nudge")]
    Task NudgePlayer([Path] string sid, [Body] NudgePlayerRequest request, CancellationToken cancellationToken = default);

    [Post("g/player/{sid}/next")]
    Task PlayAgain([Path] string sid, CancellationToken cancellationToken = default);

    [Post("g/player/{sid}/promote")]
    Task PromotePlayer([Path] string sid, [Body] PromotePlayerRequest request, CancellationToken cancellationToken = default);

    [Delete("g/player/{sid}/vote")]
    Task RecallVote([Path] string sid, CancellationToken cancellationToken = default);

    [Delete("g/player/{sid}/player")]
    Task RemovePlayer([Path] string sid, [Body] RemovePlayerRequest request, CancellationToken cancellationToken = default);

    [Delete("g/player/{sid}/votes")]
    Task ResetVotes([Path] string sid, CancellationToken cancellationToken = default);

    [Post("g/player/{sid}/spectating")]
    Task StartSpectating([Path] string sid, CancellationToken cancellationToken = default);

    [Delete("g/player/{sid}/spectating")]
    Task StopSpectating([Path] string sid, CancellationToken cancellationToken = default);

    [Put("g/player/{sid}/nickname")]
    Task UpdateNickname([Path] string sid, [Body] UpdateNicknameRequest request, CancellationToken cancellationToken = default);
}
