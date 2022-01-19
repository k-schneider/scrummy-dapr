namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

public record LogEntry(string Message)
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public long Timestamp { get; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();
}

public record Player(int PlayerId, string Nickname, bool IsHost, bool IsConnected, bool HasVoted);
