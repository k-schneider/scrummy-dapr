namespace Scrummy.AppContracts;

public record SyncGameMessage
{
    public GameSnapshot Snapshot { get; init; } = null!;

    // Needed for SignalR serialization
    private SyncGameMessage() { }

    public SyncGameMessage(GameSnapshot snapshot)
    {
        Snapshot = snapshot;
    }
}
