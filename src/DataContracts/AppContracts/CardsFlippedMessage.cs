namespace Scrummy.AppContracts;

public record CardsFlippedMessage
{
    public Dictionary<int, string> Votes { get; init; } = null!;

    // Needed for SignalR serialization
    private CardsFlippedMessage() { }

    public CardsFlippedMessage(Dictionary<int, string> votes)
    {
        Votes = votes;
    }
}
