namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

public record InitializeLobbyAction(string Nickname, Dictionary<string, GameMembership> Games);

public record CreateRoomAction(string Nickname);
public record CreateRoomSuccessAction(GameMembership Game);
public record CreateRoomFailedAction(string Error);

public record JoinRoomAction(string GameId, string Nickname);
public record JoinRoomSuccessAction(GameMembership Game);
public record JoinRoomFailedAction(string Error);

public record LeaveRoomAction(string GameId);
public record LeaveRoomSuccessAction(string GameId);
public record LeaveRoomFailedAction(string Error);
