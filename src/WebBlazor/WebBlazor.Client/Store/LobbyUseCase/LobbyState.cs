namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

[FeatureState]
public record LobbyState
{
    public string Nickname { get; init; } = string.Empty;
    public IEnumerable<Session> Sessions { get; init; } = Array.Empty<Session>();
}

public record Session
{
    public string GameId { get; init; } = string.Empty;
    public int PlayerId { get; init; }
    public string Sid { get; init; } = string.Empty;
}
