namespace Scrummy.AppContracts;

public record Game
{
    public string GameId { get; init; } = null!;
    public int GameVersion { get; init; }
    public string GamePhase { get; init; } = null!;
    public IEnumerable<Player> Players { get; init; } = null!;
    public IEnumerable<Card> Deck { get; init; } = null!;

    // Needed for serialization
    private Game() { }

    public Game(
        string gameId,
        int gameVersion,
        string gamePhase,
        IEnumerable<Player> players,
        IEnumerable<Card> deck)
    {
        GameId = gameId;
        GameVersion = gameVersion;
        GamePhase = gamePhase;
        Players = players;
        Deck = deck;
    }
};
