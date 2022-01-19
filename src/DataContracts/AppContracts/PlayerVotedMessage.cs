namespace Scrummy.AppContracts;

public record PlayerVotedMessage
{
    public int PlayerId { get; init; }

    // Needed for SignalR serialization
    private PlayerVotedMessage() { }

    public PlayerVotedMessage(int playerId)
    {
        PlayerId = playerId;
    }
}
