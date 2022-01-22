namespace Scrummy.AppContracts;

public record PlayerVoteCastMessage
{
    public int PlayerId { get; init; }
    public string? Vote { get; init; }

    // Needed for SignalR serialization
    private PlayerVoteCastMessage() { }

    public PlayerVoteCastMessage(int playerId, string? vote)
    {
        PlayerId = playerId;
        Vote = vote;
    }
}
