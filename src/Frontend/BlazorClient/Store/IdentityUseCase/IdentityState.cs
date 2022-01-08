namespace Scrummy.BlazorClient.Store.IdentityUseCase;

[FeatureState]
public record IdentityState
{
    public string Nickname { get; init; } = string.Empty;
    public string Sid { get; init; } = string.Empty;
}
