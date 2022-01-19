namespace Scrummy.AppContracts;

public record PlayerVoteCastMessage
{
    public int PlayerId { get; init; }

    // Needed for SignalR serialization
    private PlayerVoteCastMessage() { }

    public PlayerVoteCastMessage(int playerId)
    {
        PlayerId = playerId;
    }
}
