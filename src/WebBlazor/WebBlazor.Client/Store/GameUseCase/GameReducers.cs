namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

public static class GameReducers
{
    [ReducerMethod]
    public static GameState ReduceCardsFlippedAction(GameState state, CardsFlippedAction action)
    {
        var host = state.Players.First(p => p.IsHost);
        var name = host.PlayerId == state.PlayerId ? "You" : host.Nickname;

        return state with
        {
            GamePhase = "Results",
            Votes = action.Votes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value ?? null),
            Log = state.Log.Append(new LogEntry($"{name} flipped the cards."))
        };
    }

    [ReducerMethod]
    public static GameState ReduceCastVoteAction(GameState state, CastVoteAction action)
    {
        var previousVote = state.Votes.ContainsKey(state.PlayerId) ? state.Votes[state.PlayerId] : null;
        var votes = new Dictionary<int, string?>(state.Votes);
        votes[state.PlayerId] = action.Vote;

        return state with
        {
            PreviousVote = previousVote,
            Votes = votes,
            Voting = true
        };
    }

    [ReducerMethod]
    public static GameState ReduceCastVoteFailedAction(GameState state, CastVoteFailedAction _)
    {
        var votes = new Dictionary<int, string?>(state.Votes);

        if (state.PreviousVote is null)
        {
            votes.Remove(state.PlayerId);
        }
        else
        {
            votes[state.PlayerId] = state.PreviousVote;
        }

        return state with
        {
            Votes = votes,
            Voting = false
        };
    }

    [ReducerMethod]
    public static GameState ReduceCastVoteSuccessAction(GameState state, CastVoteSuccessAction _) =>
        state with
        {
            Voting = false
        };

    [ReducerMethod]
    public static GameState ReduceCloseInvitePopoverAction(GameState state, CloseInvitePopoverAction _) =>
        state with
        {
            InvitePopoverOpen = false
        };

    [ReducerMethod]
    public static GameState ReduceCloseOtherPlayerMenuAction(GameState state, CloseOtherPlayerMenuAction _) =>
        state with
        {
            OtherPlayerIdMenuOpen = null
        };

    [ReducerMethod]
    public static GameState ReduceClosePlayerPopoverAction(GameState state, ClosePlayerPopoverAction _) =>
        state with
        {
            PlayerPopoverOpen = false
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
            Connected = true,
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
    public static GameState ReduceFlipCardsAction(GameState state, FlipCardsAction _) =>
        state with
        {
            Flipping = true
        };

    [ReducerMethod]
    public static GameState ReduceFlipCardsFailedAction(GameState state, FlipCardsFailedAction _) =>
        state with
        {
            Flipping = false
        };

    [ReducerMethod]
    public static GameState ReduceFlipCardsSuccessAction(GameState state, FlipCardsSuccessAction _) =>
        state with
        {
            Flipping = false
        };

    [ReducerMethod]
    public static GameState ReduceHostChangedAction(GameState state, HostChangedAction action)
    {
        var players = state.Players.Select(p => p with
        {
            IsHost = p.PlayerId == action.PlayerId
        });

        var name = state.PlayerId == action.PlayerId ? "You" : players.First(p => p.IsHost).Nickname;
        var linkingVerb = state.PlayerId == action.PlayerId ? "are" : "is";

        return state with
        {
            Players = players,
            Log = state.Log.Append(new LogEntry($"{name} {linkingVerb} now the host."))
        };
    }

    [ReducerMethod]
    public static GameState ReduceLeaveGameAction(GameState state, LeaveGameAction _) =>
        state with
        {
            Leaving = true
        };

    [ReducerMethod]
    public static GameState ReduceLeaveGameFailedAction(GameState state, LeaveGameFailedAction _) =>
        state with
        {
            Leaving = false
        };

    [ReducerMethod]
    public static GameState ReduceLeaveGameSuccessAction(GameState state, LeaveGameSuccessAction _) =>
        state with
        {
            Leaving = false
        };

    [ReducerMethod]
    public static GameState ReduceNewVoteStartedAction(GameState state, NewVoteStartedAction _)
    {
        var host = state.Players.First(p => p.IsHost);
        var name = host.PlayerId == state.PlayerId ? "You" : host.Nickname;

        return state with
        {
            GamePhase = "Voting",
            Votes = new(),
            Log = state.Log.Append(new LogEntry($"{name} started a new round of voting."))
        };
    }

    [ReducerMethod]
    public static GameState ReduceOpenInvitePopoverAction(GameState state, OpenInvitePopoverAction _) =>
        state with
        {
            InvitePopoverOpen = true
        };

    [ReducerMethod]
    public static GameState ReduceOpenOtherPlayerMenuAction(GameState state, OpenOtherPlayerMenuAction action) =>
        state with
        {
            OtherPlayerIdMenuOpen = action.PlayerId
        };

    [ReducerMethod]
    public static GameState ReduceOpenPlayerPopoverAction(GameState state, OpenPlayerPopoverAction _) =>
        state with
        {
            PlayerPopoverOpen = true
        };

    [ReducerMethod]
    public static GameState ReducePlayAgainAction(GameState state, PlayAgainAction _) =>
        state with
        {
            PlayingAgain = true
        };

    [ReducerMethod]
    public static GameState ReducePlayAgainFailedAction(GameState state, PlayAgainFailedAction _) =>
        state with
        {
            PlayingAgain = false
        };

    [ReducerMethod]
    public static GameState ReducePlayAgainSuccessAction(GameState state, PlayAgainSuccessAction _) =>
        state with
        {
            PlayingAgain = false
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
            Players = state.Players.Append(new Player(action.PlayerId, action.Nickname, false, false)),
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

        var votes = new Dictionary<int, string?>(state.Votes);
        votes.Remove(action.PlayerId);

        return state with
        {
            Players = state.Players.Where(p => p.PlayerId != player.PlayerId),
            Votes = votes,
            Log = state.Log.Append(new LogEntry($"{player.Nickname} left the game."))
        };
    }

    [ReducerMethod]
    public static GameState ReducePlayerVoteCastAction(GameState state, PlayerVoteCastAction action)
    {
        var player = state.Players
            .Where(p => p.PlayerId == action.PlayerId)
            .First();

        var name = state.PlayerId == action.PlayerId ? "You" : player.Nickname;
        var pronoun = state.PlayerId == action.PlayerId ? "your" : "their";

        var logMessage = state.Votes.ContainsKey(action.PlayerId)
            ? $"{name} changed {pronoun} vote."
            : $"{name} voted.";

        var votes = new Dictionary<int, string?>(state.Votes);
        votes[action.PlayerId] = action.Vote;

        return state with
        {
            Votes = votes,
            Log = state.Log.Append(new LogEntry(logMessage))
        };
    }

    [ReducerMethod]
    public static GameState ReducePlayerVoteRecalledAction(GameState state, PlayerVoteRecalledAction action)
    {
        var player = state.Players
            .Where(p => p.PlayerId == action.PlayerId)
            .First();

        var name = state.PlayerId == action.PlayerId ? "You" : player.Nickname;
        var pronoun = state.PlayerId == action.PlayerId ? "your" : "their";

        var votes = new Dictionary<int, string?>(state.Votes);
        votes.Remove(action.PlayerId);

        return state with
        {
            Votes = votes,
            Log = state.Log.Append(new LogEntry($"{name} recalled {pronoun} vote."))
        };
    }

    [ReducerMethod]
    public static GameState ReduceRecallVoteAction(GameState state, RecallVoteAction action)
    {
        var previousVote = state.Votes.ContainsKey(state.PlayerId) ? state.Votes[state.PlayerId] : null;
        var votes = new Dictionary<int, string?>(state.Votes);
        votes.Remove(state.PlayerId);

        return state with
        {
            PreviousVote = previousVote,
            Votes = votes,
            Voting = true
        };
    }

    [ReducerMethod]
    public static GameState ReduceRecallVoteFailedAction(GameState state, RecallVoteFailedAction _)
    {
        var votes = new Dictionary<int, string?>(state.Votes);

        if (state.PreviousVote is null)
        {
            votes.Remove(state.PlayerId);
        }
        else
        {
            votes[state.PlayerId] = state.PreviousVote;
        }

        return state with
        {
            Votes = votes,
            Voting = false
        };
    }

    [ReducerMethod]
    public static GameState ReduceRecallVoteSuccessAction(GameState state, RecallVoteSuccessAction _) =>
        state with
        {
            Voting = false
        };

    [ReducerMethod]
    public static GameState ReduceResetVotesAction(GameState state, ResetVotesAction _) =>
        state with
        {
            ResettingVotes = true
        };

    [ReducerMethod]
    public static GameState ReduceResetVotesFailedAction(GameState state, ResetVotesFailedAction _) =>
        state with
        {
            ResettingVotes = false
        };

    [ReducerMethod]
    public static GameState ReduceResetVotesSuccessAction(GameState state, ResetVotesSuccessAction _) =>
        state with
        {
            ResettingVotes = false
        };

    [ReducerMethod]
    public static GameState ReduceSyncGameAction(GameState state, SyncGameAction action) =>
        state with
        {
            GameId = action.Snapshot.GameId,
            GamePhase = action.Snapshot.GamePhase,
            Deck = action.Snapshot.Deck,
            Players = action.Snapshot.Players.Select(p => new Player(p.PlayerId, p.Nickname, p.IsHost, p.IsConnected)),
            Votes = action.Snapshot.Votes,
            InSync = true
        };

    [ReducerMethod]
    public static GameState ReduceVotesResetAction(GameState state, VotesResetAction _)
    {
        var host = state.Players.First(p => p.IsHost);
        var name = host.PlayerId == state.PlayerId ? "You" : host.Nickname;

        return state with
        {
            Votes = new(),
            Log = state.Log.Append(new LogEntry($"{name} reset the votes."))
        };
    }
}
