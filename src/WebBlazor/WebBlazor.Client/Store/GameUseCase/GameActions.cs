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

public record SyncGameAction(GameSnapshot snapshot);
