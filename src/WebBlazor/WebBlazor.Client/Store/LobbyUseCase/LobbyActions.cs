namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

public record CreateRoomAction(string Nickname);
public record CreateRoomSuccessAction();

public record HydrateLobbyAction(string Nickname);
