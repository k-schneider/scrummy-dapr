namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

[FeatureState]
public record LobbyState
{
    public bool CreatingRoom { get; init; }
    public IEnumerable<GameSession> Games { get; init; } = Array.Empty<GameSession>();
    public bool Initialized { get; init; }
    public string Nickname { get; init; } = string.Empty;
}
