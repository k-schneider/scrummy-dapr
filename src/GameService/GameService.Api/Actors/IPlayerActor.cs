namespace Scrummy.GameService.Api.Actors;

public interface IPlayerActor : IActor
{
    Task AddConnection(string connectionId, CancellationToken cancellationToken = default);
    Task CastVote(string vote, CancellationToken cancellationToken = default);
    Task FlipCards(CancellationToken cancellationToken = default);
    Task<PlayerState> GetPlayerState(CancellationToken cancellationToken = default);
    Task<int> JoinGame(string gameId, string nickname, CancellationToken cancellationToken = default);
    Task LeaveGame(CancellationToken cancellationToken = default);
    Task NudgePlayer(int playerId, CancellationToken cancellationToken = default);
    Task PlayAgain(CancellationToken cancellationToken = default);
    Task PromotePlayer(int playerId, CancellationToken cancellationToken = default);
    Task PromoteToHost(CancellationToken cancellationToken = default);
    Task RecallVote(CancellationToken cancellationToken = default);
    Task RemoveConnection(string connectionId, CancellationToken cancellationToken = default);
    Task RemovePlayer(int playerId, CancellationToken cancellationToken = default);
    Task Reset(CancellationToken cancellationToken = default);
    Task ResetVote(CancellationToken cancellationToken = default);
    Task ResetVotes(CancellationToken cancellationToken = default);
    Task StartSpectating(CancellationToken cancellationToken = default);
    Task StopSpectating(CancellationToken cancellationToken = default);
    Task UpdateNickname(string nickname, CancellationToken cancellationToken = default);
}
