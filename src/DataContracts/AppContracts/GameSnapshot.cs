namespace Scrummy.AppContracts;

public record GameSnapshot
{
    public string GameId { get; init; } = null!;
    public IEnumerable<PlayerSnapshot> Players { get; init; } = null!;
    public IEnumerable<string> Deck { get; init; } = null!;
    public string? Vote { get; init; }

    // Needed for SignalR serialization
    private GameSnapshot() { }

    public GameSnapshot(
        string gameId,
        IEnumerable<PlayerSnapshot> players,
        IEnumerable<string> deck,
        string? vote)
    {
        GameId = gameId;
        Players = players;
        Deck = deck;
        Vote = vote;
    }
};

public record PlayerSnapshot
{
    public int PlayerId { get; init; }
    public string Nickname { get; init; } = null!;
    public bool IsHost { get; set; }
    public bool IsConnected { get; init; }
    public bool HasVoted { get; init; }

    // Needed for SignalR serialization
    private PlayerSnapshot() { }

    public PlayerSnapshot(int playerId, string nickname, bool isHost, bool isConnected, bool hasVoted)
    {
        PlayerId = playerId;
        Nickname = nickname;
        IsHost = isHost;
        IsConnected = isConnected;
        HasVoted = hasVoted;
    }
}
