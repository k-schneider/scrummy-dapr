namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

public record GameMembership
{
    public string GameId { get; init; } = null!;
    public int PlayerId { get; init; }
    public string Sid { get; init; } = null!;
    public long JoinedAt { get; init; }

    // Needed for localStorage serialization
    private GameMembership() { }

    public GameMembership(string gameId, int playerId, string sid)
    {
        GameId = gameId;
        PlayerId = playerId;
        Sid = sid;
        JoinedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
};
