namespace Scrummy.GameService.Api.Actors;

public interface IGameActor : IActor
{
    Task<(string sid, int playerId)> AddPlayer(string nickname, CancellationToken cancellationToken = default);
    Task CastVote(string sid, string vote, CancellationToken cancellationToken = default);
    Task FlipCards(string sid, CancellationToken cancellationToken = default);
    Task<GameSnapshot> GetGameSnapshot(int playerId, CancellationToken cancellationToken = default);
    Task NotifyPlayerConnected(int playerId, CancellationToken cancellationToken = default);
    Task NotifyPlayerDisconnected(int playerId, CancellationToken cancellationToken = default);
    Task PlayAgain(string sid, CancellationToken cancellationToken = default);
    Task RecallVote(string sid, CancellationToken cancellationToken = default);
    Task RemovePlayer(string sid, CancellationToken cancellationToken = default);
    Task ResetVotes(string sid, CancellationToken cancellationToken = default);
    Task StartGame(CancellationToken cancellationToken = default);
    Task UpdateNickname(string sid, string nickname, CancellationToken cancellationToken = default);
}
