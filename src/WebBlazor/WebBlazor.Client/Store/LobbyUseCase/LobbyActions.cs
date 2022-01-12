namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

public record CreateRoomAction(string Nickname);
public record CreateRoomSuccessAction(GameSession Game);
public record CreateRoomFailedAction();

public record InitializeLobbyAction(string Nickname, IEnumerable<GameSession> Games);
