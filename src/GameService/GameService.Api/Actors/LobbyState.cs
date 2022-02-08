namespace Scrummy.GameService.Api.Actors;

public class LobbyState
{
    public HashSet<string> Games { get; init; } = new();
}
