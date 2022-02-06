namespace Scrummy.AppContracts;

public record PlayerVoteCastMessage
{
    public int PlayerId { get; init; }
    public bool HadPreviousVote { get; init; }
    public string? Vote { get; init; }

    // Needed for SignalR serialization
    private PlayerVoteCastMessage() { }

    public PlayerVoteCastMessage(int playerId, bool hadPreviousVote, string? vote)
    {
        PlayerId = playerId;
        HadPreviousVote = hadPreviousVote;
        Vote = vote;
    }
}
