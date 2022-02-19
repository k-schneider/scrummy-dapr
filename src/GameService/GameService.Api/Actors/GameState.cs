namespace Scrummy.GameService.Api.Actors;

public class GameState
{
    // Needed for serialization
    private GameState() { }

    public GameState(string gameId)
    {
        GameId = gameId;
    }

    public string GameId { get; init; } = null!;
    public int GameVersion { get; set; } = 1;
    public string GameStatus { get; set; } = GameStatuses.None;
    public string GamePhase { get; set; } = GamePhases.Voting;
    public int PlayerCounter { get; set; }
    public List<Card> Deck { get; init; } = new();
    public Dictionary<int, string> Players { get; init; } = new();
}
