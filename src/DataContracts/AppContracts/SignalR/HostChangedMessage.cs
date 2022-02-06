namespace Scrummy.AppContracts.SignalR;

public record HostChangedMessage
{
    public int PlayerId { get; init; }

    // Needed for SignalR serialization
    private HostChangedMessage() { }

    public HostChangedMessage(int playerId)
    {
        PlayerId = playerId;
    }
}
