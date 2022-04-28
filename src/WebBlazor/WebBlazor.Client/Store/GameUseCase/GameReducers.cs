namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

public static class GameReducers
{
    [ReducerMethod]
    public static GameState ReduceCardsFlippedAction(GameState state, CardsFlippedAction action)
    {
        var players = state.Players.Select(p => p with {
            HasVoted = action.Votes.ContainsKey(p.PlayerId),
            Vote = action.Votes.ContainsKey(p.PlayerId) ? action.Votes[p.PlayerId] : null
        });

        var host = players.First(p => p.IsHost);
        var name = host.PlayerId == state.PlayerId ? "You" : host.Nickname;

        return state with
        {
            GamePhase = GamePhases.Results,
            Players = players,
            Log = state.Log.Append(new LogEntry($"{name} flipped the cards."))
        };
    }

    [ReducerMethod]
    public static GameState ReduceCastVoteAction(GameState state, CastVoteAction action)
    {
        var previousVote = state.Me()!.Vote;

        var players = state.Players.Select(p => p with {
            HasVoted = p.PlayerId == state.PlayerId ? true : p.HasVoted,
            Vote = p.PlayerId == state.PlayerId ? action.Vote : p.Vote
        });

        return state with
        {
            Players = players,
            PreviousVote = previousVote,
            Voting = true
        };
    }

    [ReducerMethod]
    public static GameState ReduceCastVoteFailedAction(GameState state, CastVoteFailedAction _)
    {
        var players = state.Players.Select(p => p with {
            HasVoted = p.PlayerId == state.PlayerId ? state.PreviousVote is not null : p.HasVoted,
            Vote = p.PlayerId == state.PlayerId ? state.PreviousVote : p.Vote
        });

        return state with
        {
            Players = players,
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
    public static GameState ReduceConnectToGameAction(GameState state, ConnectToGameAction action) =>
        state with
        {
            Connected = false,
            Connecting = true,
            GameId = action.GameId
        };

    [ReducerMethod]
    public static GameState ReduceConnectToGameFailedAction(GameState state, ConnectToGameFailedAction _) =>
        state with
        {
            Connecting = false
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
    public static GameState ReduceDisableNudgeAnimationAction(GameState state, DisableNudgeAnimationAction _) =>
        state with
        {
            DisableNudgeAnimation = true
        };

    [ReducerMethod]
    public static GameState ReduceDisconnectFromGameAction(GameState state, DisconnectFromGameAction _) =>
        state with
        {
            Disconnecting = true
        };

    [ReducerMethod]
    public static GameState ReduceDisconnectFromGameFailedAction(GameState state, DisconnectFromGameFailedAction _) =>
        state with
        {
            Disconnecting = false
        };

    [ReducerMethod]
    public static GameState ReduceDisconnectFromGameSuccessAction(GameState _, DisconnectFromGameSuccessAction _1) =>
        new GameState();

    [ReducerMethod]
    public static GameState ReduceEnableNudgeAnimationAction(GameState state, EnableNudgeAnimationAction _) =>
        state with
        {
            DisableNudgeAnimation = false
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
    public static GameState ReduceGameConnectionClosedAction(GameState state, GameConnectionClosedAction _) =>
        state with
        {
            ConnectionClosed = true
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
    public static GameState ReduceMuteSoundsAction(GameState state, MuteSoundsAction _) =>
        state with
        {
            MuteSounds = true
        };

    [ReducerMethod]
    public static GameState ReduceNewVoteStartedAction(GameState state, NewVoteStartedAction _)
    {
        var host = state.Players.First(p => p.IsHost);
        var name = host.PlayerId == state.PlayerId ? "You" : host.Nickname;

        var players = state.Players.Select(p => p with {
            HasVoted = false,
            Vote = null
        });

        return state with
        {
            GamePhase = GamePhases.Voting,
            Players = players,
            Log = state.Log.Append(new LogEntry($"{name} started a new round of voting."))
        };
    }

    [ReducerMethod]
    public static GameState ReduceNudgePlayerAction(GameState state, NudgePlayerAction action) =>
        state with
        {
            NudgingPlayer = action.PlayerId
        };

    [ReducerMethod]
    public static GameState ReduceNudgePlayerFailedAction(GameState state, NudgePlayerFailedAction _) =>
        state with
        {
            NudgingPlayer = null
        };

    [ReducerMethod]
    public static GameState ReduceNudgePlayerSuccessAction(GameState state, NudgePlayerSuccessAction _) =>
        state with
        {
            NudgingPlayer = null
        };

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
    public static GameState ReduceReconnectToGameAction(GameState state, ReconnectToGameAction _) =>
        state with
        {
            Reconnecting = true
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
    public static GameState ReducePlayerIsSpectatorChangedAction(GameState state, PlayerIsSpectatorChangedAction action)
    {
        var players = state.Players.Select(p => p with
        {
            IsSpectator = p.PlayerId == action.PlayerId ? action.IsSpectator : p.IsSpectator,
            Vote = p.PlayerId == action.PlayerId ? null : p.Vote,
            HasVoted = p.PlayerId == action.PlayerId ? false : p.HasVoted,
        });

        var name = state.PlayerId == action.PlayerId ? "You" : state.Players.First(p => p.PlayerId == action.PlayerId).Nickname;
        var verb = action.IsSpectator ? "started" : "stopped";

        return state with
        {
            Players = players,
            Log = state.Log.Append(new LogEntry($"{name} {verb} spectating."))
        };
    }

    [ReducerMethod]
    public static GameState ReducePlayerJoinedAction(GameState state, PlayerJoinedAction action) =>
        state with
        {
            Players = state.Players.Append(new Player(action.PlayerId, action.Nickname, false, false, false, null, false)),
            Log = state.Log.Append(new LogEntry($"{action.Nickname} joined the game."))
        };

    [ReducerMethod]
    public static GameState ReducePlayerLeftAction(GameState state, PlayerLeftAction action)
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
    public static GameState ReducePlayerNicknameChangedAction(GameState state, PlayerNicknameChangedAction action)
    {
        var players = state.Players.Select(p => p with
        {
            Nickname = p.PlayerId == action.PlayerId ? action.Nickname : p.Nickname
        });

        var name = state.PlayerId == action.PlayerId ? "You" : state.Players.First(p => p.PlayerId == action.PlayerId).Nickname;
        var pronoun = state.PlayerId == action.PlayerId ? "your" : "their";

        return state with
        {
            Players = players,
            Log = state.Log.Append(new LogEntry($"{name} changed {pronoun} nickname to {action.Nickname}."))
        };
    }

    [ReducerMethod]
    public static GameState ReducePlayerNudgedAction(GameState state, PlayerNudgedAction action)
    {
        var fromPlayer = state.Players
            .Where(p => p.PlayerId == action.FromPlayerId)
            .First();

        var toPlayer = state.Players
            .Where(p => p.PlayerId == action.ToPlayerId)
            .First();

        var fromName = state.PlayerId == fromPlayer.PlayerId ? "You" : fromPlayer.Nickname;
        var toName = state.PlayerId == toPlayer.PlayerId ? "you" : toPlayer.Nickname;

        return state with
        {
            Nudged = action.ToPlayerId == state.PlayerId ? true : state.Nudged,
            Log = state.Log.Append(new LogEntry($"{fromName} nudged {toName}."))
        };
    }

    [ReducerMethod]
    public static GameState ReducePlayerRemovedAction(GameState state, PlayerRemovedAction action)
    {
        if (state.PlayerId == action.PlayerId)
        {
            // Let effect handle redirect/disconnect
            return state;
        }

        var player = state.Players
            .Where(p => p.PlayerId == action.PlayerId)
            .First();

        return state with
        {
            Players = state.Players.Where(p => p.PlayerId != player.PlayerId),
            Log = state.Log.Append(new LogEntry($"{player.Nickname} was removed from the game."))
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

        var logMessage = action.hadPreviousVote
            ? $"{name} changed {pronoun} vote."
            : $"{name} voted.";

        var players = state.Players.Select(p => p with {
            HasVoted = p.PlayerId == action.PlayerId ? true : p.HasVoted,
            Vote = p.PlayerId == action.PlayerId ? action.Vote : p.Vote
        });

        return state with
        {
            Players = players,
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

        var players = state.Players.Select(p => p with {
            HasVoted = p.PlayerId == action.PlayerId ? false : p.HasVoted
        });

        return state with
        {
            Players = players,
            Log = state.Log.Append(new LogEntry($"{name} recalled {pronoun} vote."))
        };
    }

    [ReducerMethod]
    public static GameState ReducePromotePlayerAction(GameState state, PromotePlayerAction action) =>
        state with
        {
            PromotingPlayer = action.PlayerId
        };

    [ReducerMethod]
    public static GameState ReducePromotePlayerFailedAction(GameState state, PromotePlayerFailedAction _) =>
        state with
        {
            PromotingPlayer = null
        };

    [ReducerMethod]
    public static GameState ReducePromotePlayerSuccessAction(GameState state, PromotePlayerSuccessAction _) =>
        state with
        {
            PromotingPlayer = null
        };

    [ReducerMethod]
    public static GameState ReduceRecallVoteAction(GameState state, RecallVoteAction _)
    {
        var previousVote = state.Me()!.Vote;

        var players = state.Players.Select(p => p with {
            HasVoted = p.PlayerId == state.PlayerId ? false : p.HasVoted,
            Vote = p.PlayerId == state.PlayerId ? null : p.Vote
        });

        return state with
        {
            Players = players,
            PreviousVote = previousVote,
            RecallingVote = true
        };
    }

    [ReducerMethod]
    public static GameState ReduceRecallVoteFailedAction(GameState state, RecallVoteFailedAction _)
    {
        var players = state.Players.Select(p => p with {
            HasVoted = p.PlayerId == state.PlayerId ? state.PreviousVote is not null : p.HasVoted,
            Vote = p.PlayerId == state.PlayerId ? state.PreviousVote : p.Vote
        });

        return state with
        {
            Players = players,
            RecallingVote = false
        };
    }

    [ReducerMethod]
    public static GameState ReduceRecallVoteSuccessAction(GameState state, RecallVoteSuccessAction _) =>
        state with
        {
            RecallingVote = false
        };

    [ReducerMethod]
    public static GameState ReduceReceiveGameStateAction(GameState state, ReceiveGameStateAction action) =>
        state with
        {
            GameId = action.Game.GameId,
            GamePhase = action.Game.GamePhase,
            GameVersion = action.Game.GameVersion,
            Deck = action.Game.Deck,
            Players = action.Game.Players
        };

    [ReducerMethod]
    public static GameState ReduceRemovePlayerAction(GameState state, RemovePlayerAction action) =>
        state with
        {
            RemovingPlayer = action.PlayerId
        };

    [ReducerMethod]
    public static GameState ReduceRemovePlayerFailedAction(GameState state, RemovePlayerFailedAction _) =>
        state with
        {
            RemovingPlayer = null
        };

    [ReducerMethod]
    public static GameState ReduceRemovePlayerSuccessAction(GameState state, RemovePlayerSuccessAction _) =>
        state with
        {
            PromotingPlayer = null
        };

    [ReducerMethod]
    public static GameState ReduceResetNudgedAction(GameState state, ResetNudgedAction _) =>
        state with
        {
            Nudged = false
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
    public static GameState ReduceStartSpectatingAction(GameState state, StartSpectatingAction _) =>
        state with
        {
            SpectateChanging = true
        };

    [ReducerMethod]
    public static GameState ReduceStartSpectatingFailedAction(GameState state, StartSpectatingFailedAction _) =>
        state with
        {
            SpectateChanging = false
        };

    [ReducerMethod]
    public static GameState ReduceStartSpectatingSuccessAction(GameState state, StartSpectatingSuccessAction _) =>
        state with
        {
            SpectateChanging = false
        };

    [ReducerMethod]
    public static GameState ReduceStopSpectatingAction(GameState state, StopSpectatingAction _) =>
        state with
        {
            SpectateChanging = true
        };

    [ReducerMethod]
    public static GameState ReduceStopSpectatingFailedAction(GameState state, StopSpectatingFailedAction _) =>
        state with
        {
            SpectateChanging = false
        };

    [ReducerMethod]
    public static GameState ReduceStopSpectatingSuccessAction(GameState state, StopSpectatingSuccessAction _) =>
        state with
        {
            SpectateChanging = false
        };

    [ReducerMethod]
    public static GameState ReduceUnmuteSoundsAction(GameState state, UnmuteSoundsAction _) =>
        state with
        {
            MuteSounds = false
        };

    [ReducerMethod]
    public static GameState ReduceUpdateNicknameAction(GameState state, UpdateNicknameAction _) =>
        state with
        {
            UpdatingNickname = true
        };

    [ReducerMethod]
    public static GameState ReduceUpdateNicknameFailedAction(GameState state, UpdateNicknameFailedAction _) =>
        state with
        {
            UpdatingNickname = false
        };

    [ReducerMethod]
    public static GameState ReduceUpdateNicknameSuccessAction(GameState state, UpdateNicknameSuccessAction _) =>
        state with
        {
            UpdatingNickname = false
        };

    [ReducerMethod]
    public static GameState ReduceVotesResetAction(GameState state, VotesResetAction _)
    {
        var host = state.Players.First(p => p.IsHost);
        var name = host.PlayerId == state.PlayerId ? "You" : host.Nickname;

        var players = state.Players.Select(p => p with {
            HasVoted = false,
            Vote = null
        });

        return state with
        {
            Players = players,
            Log = state.Log.Append(new LogEntry($"{name} reset the votes."))
        };
    }
}
