namespace Scrummy.GameService.Api.Actors;

public class GameState
{
    public int Version { get; set; } = 1;
    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
    public GameStatus GameStatus { get; set; } = GameStatus.New;
    // todo: deck
    // todo: players
    // todo: owner
    // todo: votes
}
