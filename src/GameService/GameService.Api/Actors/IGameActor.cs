namespace Scrummy.GameService.Api.Actors;

public interface IGameActor : IActor
{
    Task<(int PlayerId, int PlayerCount)> AddPlayer(string sid, CancellationToken cancellationToken = default);
    Task BeginVoting(CancellationToken cancellationToken = default);
    Task EndGame(CancellationToken cancellationToken = default);
    Task<GameState> GetGameState(CancellationToken cancellationToken = default);
    Task<int> RemovePlayer(string sid, CancellationToken cancellationToken = default);
    Task Reset(CancellationToken cancellationToken = default);
    Task SlideInactivityReminder(CancellationToken cancellationToken = default);
    Task ShowResults(CancellationToken cancellationToken = default);
    Task StartGame(IEnumerable<Card> deck, CancellationToken cancellationToken = default);
}
