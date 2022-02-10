namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

public record InitializeLobbyAction(string Nickname, Dictionary<string, GameMembership> Games);

public record CreateGameAction(string Nickname, IEnumerable<Card> Deck);
public record CreateGameSuccessAction(GameMembership Game);
public record CreateGameFailedAction(string Error);

public record EnsureGameExistsAction(string GameId);
public record EnsureGameExistsSuccessAction(bool Exists);
public record EnsureGameExistsFailedAction(string Error);

public record ForgetGameAction(string GameId);

public record JoinGameAction(string GameId, string Nickname);
public record JoinGameSuccessAction(GameMembership Game);
public record JoinGameFailedAction(string Error);
