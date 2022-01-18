namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

public record ConnectToGameAction(string GameId);
public record ConnectToGameSuccessAction();
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

public record SyncGameAction(GameSnapshot Snapshot);
