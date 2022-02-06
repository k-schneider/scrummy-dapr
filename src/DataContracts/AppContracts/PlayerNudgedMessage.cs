namespace Scrummy.AppContracts;

public record PlayerNudgedMessage
{
    public int FromPlayerId { get; init; }
    public int ToPlayerId { get; init; }

    // Needed for SignalR serialization
    private PlayerNudgedMessage() { }

    public PlayerNudgedMessage(int fromPlayerId, int toPlayerId)
    {
        FromPlayerId = fromPlayerId;
        ToPlayerId = toPlayerId;
    }
}
