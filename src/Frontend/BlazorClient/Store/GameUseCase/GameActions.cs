namespace Scrummy.BlazorClient.Store.GameUseCase;

public record JoinGameAction(string GameId);
public record JoinGameSuccessAction();
public record JoinGameFailedAction();

public record LeaveGameAction();
public record LeaveGameSuccessAction();
public record LeaveGameFailedAction();
