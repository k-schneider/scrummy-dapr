namespace Scrummy.AppContracts;

public record PlayerRemovedMessage
{
    public int PlayerId { get; init; }

    // Needed for SignalR serialization
    private PlayerRemovedMessage() { }

    public PlayerRemovedMessage(int playerId)
    {
        PlayerId = playerId;
    }
}
