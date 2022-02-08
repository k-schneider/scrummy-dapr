namespace Scrummy.AppContracts.SignalR;

public record ReceiveGameStateMessage
{
    public Game Game { get; init; } = null!;

    // Needed for SignalR serialization
    private ReceiveGameStateMessage() { }

    public ReceiveGameStateMessage(Game game)
    {
        Game = game;
    }
}
