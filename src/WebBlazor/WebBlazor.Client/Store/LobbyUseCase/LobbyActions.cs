namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

public record InitializeLobbyAction(string Nickname, Dictionary<string, GameSession> Games);

public record CreateRoomAction(string Nickname);
public record CreateRoomSuccessAction(GameSession Game);
public record CreateRoomFailedAction(string Error);

public record JoinRoomAction(string GameId, string Nickname);
public record JoinRoomSuccessAction(GameSession Game);
public record JoinRoomFailedAction(string Error);

public record LeaveRoomAction(string GameId);
public record LeaveRoomSuccessAction(GameSession Game);
public record LeaveRoomFailedAction(string Error);
