namespace Scrummy.AppContracts;

public record Game
{
    public string GameId { get; init; } = null!;
    public int GameVersion { get; init; }
    public string GamePhase { get; init; } = null!;
    public IEnumerable<Player> Players { get; init; } = null!;
    public IEnumerable<string> Deck { get; init; } = null!;
    public Dictionary<int, string?> Votes { get; init; } = null!;

    // Needed for serialization
    private Game() { }

    public Game(
        string gameId,
        int gameVersion,
        string gamePhase,
        IEnumerable<Player> players,
        IEnumerable<string> deck,
        Dictionary<int, string?> votes)
    {
        GameId = gameId;
        GameVersion = gameVersion;
        GamePhase = gamePhase;
        Players = players;
        Deck = deck;
        Votes = votes;
    }
};
