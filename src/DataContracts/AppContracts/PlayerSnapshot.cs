namespace Scrummy.AppContracts;

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
