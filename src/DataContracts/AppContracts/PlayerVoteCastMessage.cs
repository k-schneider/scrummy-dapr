namespace Scrummy.AppContracts;

public record PlayerVoteCastMessage
{
    public int PlayerId { get; init; }
    public string? Vote { get; init; }
    public string? PreviousVote { get; init; }

    // Needed for SignalR serialization
    private PlayerVoteCastMessage() { }

    public PlayerVoteCastMessage(int playerId, string? vote, string? previousVote)
    {
        PlayerId = playerId;
        Vote = vote;
        PreviousVote = previousVote;
    }
}
