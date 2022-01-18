namespace Scrummy.AppContracts;

public record PlayerJoinedGameMessage
{
    public int PlayerId { get; init; }
    public string Nickname { get; init; } = null!;

    // Needed for SignalR serialization
    private PlayerJoinedGameMessage() { }

    public PlayerJoinedGameMessage(int playerId, string nickname)
    {
        PlayerId = playerId;
        Nickname = nickname;
    }
}
