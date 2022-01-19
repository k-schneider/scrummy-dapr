namespace Scrummy.AppContracts;

public record PlayerVoteRecalledMessage
{
    public int PlayerId { get; init; }

    // Needed for SignalR serialization
    private PlayerVoteRecalledMessage() { }

    public PlayerVoteRecalledMessage(int playerId)
    {
        PlayerId = playerId;
    }
}
