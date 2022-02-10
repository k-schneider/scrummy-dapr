namespace Scrummy.GameService.Api.Actors;

public class GameState
{
    public int GameVersion { get; set; } = 1;
    public string GameStatus { get; set; } = GameStatuses.None;
    public string GamePhase { get; set; } = GamePhases.Voting;
    public int PlayerCounter { get; set; }
    public List<PlayerState> Players { get; init; } = new();
    public Dictionary<int, string> Votes { get; init; } = new();
    public List<Card> Deck { get; init; } = new()
    {
        new Card("0", 0),
        new Card("1/2", 0.5),
        new Card("1", 1),
        new Card("2", 2),
        new Card("3", 3),
        new Card("5", 5),
        new Card("8", 8),
        new Card("13", 13),
        new Card("20", 20),
        new Card("40", 40),
        new Card("100", 100),
        new Card("Infinite", image: "img/infinite.png"),
        new Card("?"),
        new Card("Coffee", image: "img/coffee.png"),
    };
}
