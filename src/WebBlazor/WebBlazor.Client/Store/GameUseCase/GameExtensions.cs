public static class GameExtensions
{
    public static bool AllVotesIn(this GameState state)
    {
        return state.Players.All(p => !p.IsConnected || p.IsSpectator || p.HasVoted);
    }

    public static bool GetPlayerHasVoted(this GameState state, int playerId)
    {
        return state.Players.First(p => p.PlayerId == playerId).HasVoted;
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
        return state.Players.First(p => p.PlayerId == state.PlayerId).Vote;
    }

    public static bool TryGetPlayerCard(this GameState state, int playerId, out Card? card)
    {
        var vote = state.Players.First(p => p.PlayerId == playerId).Vote;

        if (vote is not null)
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
