namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

[FeatureState]
public record GameState
{
    public bool Connecting { get; init; }
    public bool Connected { get; init; }
    public bool ConnectionClosed { get; init; }
    public IEnumerable<Card> Deck { get; init; } = Enumerable.Empty<Card>();
    public bool DisableNudgeAnimation { get; init; }
    public bool Disconnecting { get; init; }
    public bool Flipping { get; init; }
    public string? GameId { get; init; }
    public string? GamePhase { get; init; }
    public int GameVersion { get; init; }
    public bool InvitePopoverOpen { get; init; }
    public bool Leaving { get; init; }
    public IEnumerable<LogEntry> Log { get; init; } = Enumerable.Empty<LogEntry>();
    public bool MuteSounds { get; init; }
    public bool Nudged { get; init; }
    public int? NudgingPlayer { get; init ; }
    public int? OtherPlayerIdMenuOpen { get; init; }
    public int PlayerId { get; init; }
    public bool PlayerPopoverOpen { get; init; }
    public IEnumerable<Player> Players { get; init; } = Enumerable.Empty<Player>();
    public bool PlayingAgain { get; init; }
    public string? PreviousVote { get; init; }
    public int? PromotingPlayer { get; init; }
    public bool RecallingVote { get; init; }
    public bool Reconnecting { get; init; }
    public int? RemovingPlayer { get; init; }
    public bool ResettingVotes { get; init; }
    public string? Sid { get; init; }
    public bool SpectateChanging { get; init; }
    public bool UpdatingNickname { get; init; }
    public Dictionary<int, string?> Votes { get; init; } = new();
    public bool Voting { get; init; }
}
