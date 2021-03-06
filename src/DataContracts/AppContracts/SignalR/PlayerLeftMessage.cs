namespace Scrummy.AppContracts.SignalR;

public record PlayerLeftMessage
{
    public int PlayerId { get; init; }

    // Needed for SignalR serialization
    private PlayerLeftMessage() { }

    public PlayerLeftMessage(int playerId)
    {
        PlayerId = playerId;
    }
}
