namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

[FeatureState]
public record GameState
{
    public bool Connecting { get; init; }
    public bool Connected { get; init;}
    public IEnumerable<string> Deck { get; init; } = Enumerable.Empty<string>();
    public bool Disconnecting { get; init; }
    public bool Flipping { get; init; }
    public string GameId { get; init; } = string.Empty;
    public bool InSync { get; init; }
    public bool Leaving { get; init; }
    public IEnumerable<LogEntry> Log { get; init; } = Enumerable.Empty<LogEntry>();
    public int PlayerId { get; init; }
    public IEnumerable<Player> Players { get; init; } = Enumerable.Empty<Player>();
    public string GamePhase { get; init; } = string.Empty;  // Voting, Results
    public Dictionary<int, string?> Votes { get; init; } = new();
    public string? PreviousVote { get; init; }
    public string Sid { get; init; } = string.Empty;
    public bool Voting { get; init; }
}
