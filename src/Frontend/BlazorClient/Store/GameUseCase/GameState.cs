namespace Scrummy.BlazorClient.Store.GameUseCase;

[FeatureState]
public record GameState
{
    public bool Joining { get; init; }
    public bool Leaving { get; init; }
}
