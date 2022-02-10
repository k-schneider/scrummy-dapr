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
        new Card(numericValue: 0),
        new Card("1/2", 0.5),
        new Card(numericValue: 1),
        new Card(numericValue: 2),
        new Card(numericValue: 3),
        new Card(numericValue: 5),
        new Card(numericValue: 8),
        new Card(numericValue: 13),
        new Card(numericValue: 20),
        new Card(numericValue: 40),
        new Card(numericValue: 100),
        new Card(image: "img/infinity.png"),
        new Card("?"),
        new Card(image: "img/coffee.png"),
    };
}
