namespace Scrummy.AppContracts.SignalR;

public record PlayerIsSpectatorChangedMessage
{
    public int PlayerId { get; init; }
    public bool IsSpectator { get; init; }

    // Needed for SignalR serialization
    private PlayerIsSpectatorChangedMessage() { }

    public PlayerIsSpectatorChangedMessage(int playerId, bool isSpectator)
    {
        PlayerId = playerId;
        IsSpectator = isSpectator;
    }
}
