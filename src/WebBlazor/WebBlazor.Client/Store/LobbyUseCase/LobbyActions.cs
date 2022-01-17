namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

public record InitializeLobbyAction(string Nickname, Dictionary<string, GameMembership> Games);

public record CreateGameAction(string Nickname);
public record CreateGameSuccessAction(GameMembership Game);
public record CreateGameFailedAction(string Error);

public record ForgetGameAction(string GameId);

public record JoinGameAction(string GameId, string Nickname);
public record JoinGameSuccessAction(GameMembership Game);
public record JoinGameFailedAction(string Error);
