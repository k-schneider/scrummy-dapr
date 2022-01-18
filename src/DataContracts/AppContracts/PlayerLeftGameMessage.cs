namespace Scrummy.AppContracts;

public record PlayerLeftGameMessage
{
    public int PlayerId { get; init; }

    // Needed for SignalR serialization
    private PlayerLeftGameMessage() { }

    public PlayerLeftGameMessage(int playerId)
    {
        PlayerId = playerId;
    }
}
