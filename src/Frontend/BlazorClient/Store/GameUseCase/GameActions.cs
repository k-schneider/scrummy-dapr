namespace Scrummy.BlazorClient.Store.GameUseCase;

public record JoinGameAction(string GameId);
public record JoinGameSuccessAction();

public record LeaveGameAction();
public record LeaveGameSuccessAction();
