namespace Scrummy.AppContracts;

public record GameHostChangedMessage
{
    public int PlayerId { get; init; }

    // Needed for SignalR serialization
    private GameHostChangedMessage() { }

    public GameHostChangedMessage(int playerId)
    {
        PlayerId = playerId;
    }
}
