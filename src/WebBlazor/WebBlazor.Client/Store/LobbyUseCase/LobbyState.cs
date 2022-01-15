namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

[FeatureState]
public record LobbyState
{
    public bool CreatingRoom { get; init; }
    public Dictionary<string, GameSession> Games { get; init; } = new();
    public bool Initialized { get; init; }
    public bool JoiningRoom { get; init; }
    public bool LeavingRoom { get; init; }
    public string Nickname { get; init; } = string.Empty;
}
