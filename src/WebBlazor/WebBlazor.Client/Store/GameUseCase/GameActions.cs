namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

public record CastVoteAction(string Vote);
public record CastVoteSuccessAction();
public record CastVoteFailedAction(string Error);

public record RecallVoteAction();
public record RecallVoteSuccessAction();
public record RecallVoteFailedAction(string Error);

public record ConnectToGameAction(string GameId);
public record ConnectToGameSuccessAction(string Sid, int PlayerId);
public record ConnectToGameFailedAction(string Error);

public record DisconnectFromGameAction();
public record DisconnectFromGameSuccessAction();
public record DisconnectFromGameFailedAction(string Error);

public record LeaveGameAction(string GameId);
public record LeaveGameSuccessAction(string GameId);
public record LeaveGameFailedAction(string Error);

public record PlayerConnectedAction(int PlayerId);
public record PlayerDisconnectedAction(int PlayerId);

public record PlayerJoinedGameAction(int PlayerId, string Nickname);

public record PlayerLeftGameAction(int PlayerId);

public record PlayerVoteCastAction(int PlayerId);
public record PlayerVoteRecalledAction(int PlayerId);

public record SyncGameAction(GameSnapshot Snapshot);
public record SyncVoteAction(string? Vote);
