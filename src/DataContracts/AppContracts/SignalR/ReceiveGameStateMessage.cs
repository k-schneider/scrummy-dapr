namespace Scrummy.AppContracts.SignalR;

public record ReceiveGameStateMessage
{
    public GameSnapshot Snapshot { get; init; } = null!;

    // Needed for SignalR serialization
    private ReceiveGameStateMessage() { }

    public ReceiveGameStateMessage(GameSnapshot snapshot)
    {
        Snapshot = snapshot;
    }
}
