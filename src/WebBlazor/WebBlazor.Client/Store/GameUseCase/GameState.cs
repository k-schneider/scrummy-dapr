namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

[FeatureState]
public record GameState
{
    public bool Connecting { get; init; }
    public bool Disconnecting { get; init; }
    public bool Leaving { get; init; }
    public IEnumerable<Player> Players { get; init; } = Enumerable.Empty<Player>();
}
