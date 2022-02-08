namespace Scrummy.GameService.Api.Actors;

public class GameState
{
    public int GameVersion { get; set; } = 1;
    public string GameStatus { get; set; } = GameStatuses.None;
    public string GamePhase { get; set; } = GamePhases.Voting;
    public int PlayerCounter { get; set; }
    public List<PlayerState> Players { get; init; } = new();
    public Dictionary<int, string> Votes { get; init; } = new();
    public HashSet<string> Deck { get; init; } = new()
    {
        "0",
        "1",
        "2",
        "3",
        "5",
        "8",
        "13",
        "20",
        "40",
        "100",
        "?"
    };
}
