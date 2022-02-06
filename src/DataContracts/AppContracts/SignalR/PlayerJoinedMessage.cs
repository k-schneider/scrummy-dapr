namespace Scrummy.AppContracts.SignalR;

public record PlayerJoinedMessage
{
    public int PlayerId { get; init; }
    public string Nickname { get; init; } = null!;

    // Needed for SignalR serialization
    private PlayerJoinedMessage() { }

    public PlayerJoinedMessage(int playerId, string nickname)
    {
        PlayerId = playerId;
        Nickname = nickname;
    }
}
