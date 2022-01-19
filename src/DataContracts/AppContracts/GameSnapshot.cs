namespace Scrummy.AppContracts;

public record GameSnapshot
{
    public string GameId { get; init; } = null!;
    public IEnumerable<PlayerSnapshot> Players { get; init; } = null!;
    public IEnumerable<string> Deck { get; init; } = null!;

    // Needed for SignalR serialization
    private GameSnapshot() { }

    public GameSnapshot(
        string gameId,
        IEnumerable<PlayerSnapshot> players,
        IEnumerable<string> deck)
    {
        GameId = gameId;
        Players = players;
        Deck = deck;
    }
};

public record PlayerSnapshot
{
    public int PlayerId { get; init; }
    public string Nickname { get; init; } = null!;
    public bool IsHost { get; set; }
    public bool IsConnected { get; init; }

    // Needed for SignalR serialization
    private PlayerSnapshot() { }

    public PlayerSnapshot(int playerId, string nickname, bool isHost, bool isConnected)
    {
        PlayerId = playerId;
        Nickname = nickname;
        IsHost = isHost;
        IsConnected = isConnected;
    }
}
