namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

[FeatureState]
public record LobbyState
{
    public bool CreatingGame { get; init; }
    public Dictionary<string, GameMembership> Games { get; init; } = new();
    public bool Initialized { get; init; }
    public bool JoiningGame { get; init; }
    public string Nickname { get; init; } = string.Empty;
}
