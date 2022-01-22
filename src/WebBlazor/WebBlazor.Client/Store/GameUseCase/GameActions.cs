namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

public record CardsFlippedAction(Dictionary<int, string> Votes);

public record CastVoteAction(string Vote);
public record CastVoteSuccessAction();
public record CastVoteFailedAction(string Error);

public record ConnectToGameAction(string GameId);
public record ConnectToGameSuccessAction(string Sid, int PlayerId);
public record ConnectToGameFailedAction(string Error);

public record DisconnectFromGameAction();
public record DisconnectFromGameSuccessAction();
public record DisconnectFromGameFailedAction(string Error);

public record FlipCardsAction();
public record FlipCardsSuccessAction();
public record FlipCardsFailedAction(string Error);

public record HostChangedAction(int PlayerId);

public record LeaveGameAction();
public record LeaveGameSuccessAction(string GameId);
public record LeaveGameFailedAction(string Error);

public record PlayerConnectedAction(int PlayerId);
public record PlayerDisconnectedAction(int PlayerId);

public record PlayerJoinedGameAction(int PlayerId, string Nickname);

public record PlayerLeftGameAction(int PlayerId);

public record PlayerVoteCastAction(int PlayerId, string? Vote);
public record PlayerVoteRecalledAction(int PlayerId);

public record RecallVoteAction();
public record RecallVoteSuccessAction();
public record RecallVoteFailedAction(string Error);

public record ResetVotesAction();
public record ResetVotesSuccessAction();
public record ResetVotesFailedAction(string Error);

public record SyncGameAction(GameSnapshot Snapshot);

public record VotesResetAction();