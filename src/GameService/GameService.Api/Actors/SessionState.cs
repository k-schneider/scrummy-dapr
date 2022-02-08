namespace Scrummy.GameService.Api.Actors;

public class SessionState
{
    public HashSet<string> ConnectionIds { get; init; } = new HashSet<string>();
    public int PlayerId { get; set; }
    public string? GameId { get; set; }
}
