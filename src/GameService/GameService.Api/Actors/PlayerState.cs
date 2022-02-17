namespace Scrummy.GameService.Api.Actors;

public class PlayerState
{
    // Needed for serialization
    private PlayerState() { }

    public PlayerState(string sid)
    {
        Sid = sid;
    }

    public string Sid { get; init; } = null!;
    public HashSet<string> ConnectionIds { get; init; } = new HashSet<string>();
    public int PlayerId { get; set; }
    public string? GameId { get; set; }
    public string? Nickname { get; set; }
    public string? Vote { get; set; }
    public bool IsHost { get; set; }
    public bool IsSpectator { get; set; }
}
