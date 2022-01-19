namespace Scrummy.AppContracts;

public record SyncVoteMessage
{
    public string? Vote { get; init; }

    // Needed for SignalR serialization
    private SyncVoteMessage() { }

    public SyncVoteMessage(string? vote)
    {
        Vote = vote;
    }
}
