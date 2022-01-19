namespace Scrummy.AppContracts;

public record VoteRecordedMessage
{
    public string Vote { get; init; } = null!;

    // Needed for SignalR serialization
    private VoteRecordedMessage() { }

    public VoteRecordedMessage(string vote)
    {
        Vote = vote;
    }
}
