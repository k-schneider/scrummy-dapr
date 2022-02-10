namespace Scrummy.GameService.Api.Actors;

public interface IGameActor : IActor
{
    Task<(string sid, int playerId)> AddPlayer(string nickname, CancellationToken cancellationToken = default);
    Task CastVote(string sid, string vote, CancellationToken cancellationToken = default);
    Task FlipCards(string sid, CancellationToken cancellationToken = default);
    Task<Game> GetGameState(int playerId, CancellationToken cancellationToken = default);
    Task LeaveGame(string sid, CancellationToken cancellationToken = default);
    Task NotifyPlayerConnected(int playerId, CancellationToken cancellationToken = default);
    Task NotifyPlayerDisconnected(int playerId, CancellationToken cancellationToken = default);
    Task NudgePlayer(string sid, int playerId, CancellationToken cancellationToken = default);
    Task PlayAgain(string sid, CancellationToken cancellationToken = default);
    Task PromotePlayer(string sid, int playerId, CancellationToken cancellationToken = default);
    Task RecallVote(string sid, CancellationToken cancellationToken = default);
    Task RemovePlayer(string sid, int playerId, CancellationToken cancellationToken = default);
    Task ResetGame(CancellationToken cancellationToken = default);
    Task ResetVotes(string sid, CancellationToken cancellationToken = default);
    Task StartGame(CancellationToken cancellationToken = default);
    Task StartSpectating(string sid, CancellationToken cancellationToken = default);
    Task StopSpectating(string sid, CancellationToken cancellationToken = default);
    Task UpdateNickname(string sid, string nickname, CancellationToken cancellationToken = default);
}
