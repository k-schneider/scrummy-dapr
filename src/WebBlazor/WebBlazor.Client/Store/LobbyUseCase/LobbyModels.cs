namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

public record GameMembership(string GameId, int PlayerId, string Sid)
{
    public long JoinedAt { get; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();
};
