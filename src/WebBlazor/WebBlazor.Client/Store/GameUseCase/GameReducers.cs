namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

public static class GameReducers
{
    [ReducerMethod]
    public static GameState ReduceCastVoteAction(GameState state, CastVoteAction action) =>
        state with
        {
            PreviousVote = state.Vote,
            Vote = action.Vote,
            Voting = true
        };

    [ReducerMethod]
    public static GameState ReduceCastVoteFailedAction(GameState state, CastVoteFailedAction _) =>
        state with
        {
            Vote = state.PreviousVote,
            Voting = false
        };

    [ReducerMethod]
    public static GameState ReduceCastVoteSuccessAction(GameState state, CastVoteSuccessAction _) =>
        state with
        {
            Voting = false
        };

    [ReducerMethod]
    public static GameState ReduceConnectToGameAction(GameState state, ConnectToGameAction _) =>
        state with
        {
            Connecting = true
        };

    [ReducerMethod]
    public static GameState ReduceConnectToGameSuccessAction(GameState state, ConnectToGameSuccessAction action) =>
        state with
        {
            Connecting = false,
            Sid = action.Sid,
            PlayerId = action.PlayerId
        };

    [ReducerMethod]
    public static GameState ReduceConnectToGameFailedAction(GameState state, ConnectToGameFailedAction _) =>
        state with
        {
            Connecting = false
        };

    [ReducerMethod]
    public static GameState ReduceDisconnectFromGameAction(GameState state, DisconnectFromGameAction _) =>
        state with
        {
            Disconnecting = true
        };

    [ReducerMethod]
    public static GameState ReduceDisconnectFromGameSuccessAction(GameState _, DisconnectFromGameSuccessAction _1) =>
        new GameState();

    [ReducerMethod]
    public static GameState ReduceDisconnectFromGameFailedAction(GameState state, DisconnectFromGameFailedAction _) =>
        state with
        {
            Disconnecting = false
        };

    [ReducerMethod]
    public static GameState ReduceLeaveGameAction(GameState state, LeaveGameAction action) =>
        state with
        {
            Leaving = true
        };

    [ReducerMethod]
    public static GameState ReduceLeaveGameFailedAction(GameState state, LeaveGameFailedAction action) =>
        state with
        {
            Leaving = false
        };

    [ReducerMethod]
    public static GameState ReduceLeaveGameSuccessAction(GameState state, LeaveGameSuccessAction action) =>
        state with
        {
            Leaving = false
        };

    [ReducerMethod]
    public static GameState ReducePlayerConnectedAction(GameState state, PlayerConnectedAction action)
    {
        var player = state.Players
            .Where(p => p.PlayerId == action.PlayerId)
            .First() with
            {
                IsConnected = true
            };

        return state with
        {
            Players = state.Players
                .Where(p => p.PlayerId != player.PlayerId)
                .Append(player),
            Log = state.Log.Append(new LogEntry($"{player.Nickname} connected."))
        };
    }

    [ReducerMethod]
    public static GameState ReducePlayerDisconnectedAction(GameState state, PlayerDisconnectedAction action)
    {
        var player = state.Players
            .Where(p => p.PlayerId == action.PlayerId)
            .First() with
            {
                IsConnected = false
            };

        return state with
        {
            Players = state.Players
                .Where(p => p.PlayerId != player.PlayerId)
                .Append(player),
            Log = state.Log.Append(new LogEntry($"{player.Nickname} disconnected."))
        };
    }

    [ReducerMethod]
    public static GameState ReducePlayerJoinedGameAction(GameState state, PlayerJoinedGameAction action) =>
        state with
        {
            Players = state.Players.Append(new Player(action.PlayerId, action.Nickname, false, false, false)),
            Log = state.Log.Append(new LogEntry($"{action.Nickname} joined the game."))
        };

    [ReducerMethod]
    public static GameState ReducePlayerLeftGameAction(GameState state, PlayerLeftGameAction action)
    {
        if (state.PlayerId == action.PlayerId)
        {
            // Player left on another tab, let effect handle redirect/disconnect
            return state;
        }

        var player = state.Players
            .Where(p => p.PlayerId == action.PlayerId)
            .First();

        return state with
        {
            Players = state.Players.Where(p => p.PlayerId != player.PlayerId),
            Log = state.Log.Append(new LogEntry($"{player.Nickname} left the game."))
        };
    }

    [ReducerMethod]
    public static GameState ReducePlayerVotedAction(GameState state, PlayerVotedAction action)
    {
        var player = state.Players
            .Where(p => p.PlayerId == action.PlayerId)
            .First();

        var newPlayer = player with
        {
            HasVoted = true
        };

        return state with
        {
            Players = state.Players
                .Where(p => p.PlayerId != player.PlayerId)
                .Append(newPlayer),
            Log = state.Log.Append(player.HasVoted
                ? new LogEntry($"{player.Nickname} changed their vote.")
                : new LogEntry($"{player.Nickname} voted."))
        };
    }

    [ReducerMethod]
    public static GameState ReduceSyncGameAction(GameState state, SyncGameAction action) =>
        state with
        {
            GameId = action.Snapshot.GameId,
            Deck = action.Snapshot.Deck,
            Players = action.Snapshot.Players.Select(p => new Player(p.PlayerId, p.Nickname, p.IsHost, p.IsConnected, p.HasVoted)),
            Vote = action.Snapshot.Vote
        };

    [ReducerMethod]
    public static GameState ReduceVoteRecordedAction(GameState state, VoteRecordedAction action)
    {
        var player = state.Players
            .Where(p => p.PlayerId == state.PlayerId)
            .First();

        var newPlayer = player with
        {
            HasVoted = true
        };

        return state with
        {
            Vote = action.Vote,
            Players = state.Players
                .Where(p => p.PlayerId != player.PlayerId)
                .Append(newPlayer),
            Log = state.Log.Append(player.HasVoted
                ? new LogEntry($"You changed your vote.")
                : new LogEntry($"You voted."))
        };
    }
}
