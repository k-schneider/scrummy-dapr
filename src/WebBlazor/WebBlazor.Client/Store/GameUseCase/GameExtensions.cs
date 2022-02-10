public static class GameExtensions
{
    public static bool AllVotesIn(this GameState state)
    {
        return state.Votes.Any() && state.Votes.Count == state.Players.Where(p => !p.IsSpectator).Count();
    }

    public static bool IsHost(this GameState state)
    {
        return state.Me()?.IsHost == true;
    }

    public static Player? Me(this GameState state)
    {
        return state.Players.FirstOrDefault(p => p.PlayerId == state.PlayerId);
    }

    public static string? MyVote(this GameState state)
    {
        state.Votes.TryGetValue(state.PlayerId, out var vote);
        return vote;
    }

    public static bool TryGetPlayerCard(this GameState state, int playerId, out Card? card)
    {
        if (state.Votes.TryGetValue(playerId, out var vote))
        {
            card = state.Deck.FirstOrDefault(c => c.Id == vote);
        }
        else
        {
            card = null;
        }

        return card is not null;
    }
}
