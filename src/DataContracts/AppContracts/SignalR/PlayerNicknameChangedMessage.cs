namespace Scrummy.AppContracts.SignalR;

public record PlayerNicknameChangedMessage
{
    public int PlayerId { get; init; }
    public string Nickname { get; init; } = null!;

    // Needed for SignalR serialization
    private PlayerNicknameChangedMessage() { }

    public PlayerNicknameChangedMessage(int playerId, string nickname)
    {
        PlayerId = playerId;
        Nickname = nickname;
    }
}
