namespace Scrummy.AppContracts.SignalR;

public record PlayerDisconnectedMessage
{
    public int PlayerId { get; init; }

    // Needed for SignalR serialization
    private PlayerDisconnectedMessage() { }

    public PlayerDisconnectedMessage(int playerId)
    {
        PlayerId = playerId;
    }
}
