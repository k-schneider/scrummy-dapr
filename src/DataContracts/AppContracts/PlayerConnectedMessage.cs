namespace Scrummy.AppContracts;

public record PlayerConnectedMessage
{
    public int PlayerId { get; init; }

    // Needed for SignalR serialization
    private PlayerConnectedMessage() { }

    public PlayerConnectedMessage(int playerId)
    {
        PlayerId = playerId;
    }
}
