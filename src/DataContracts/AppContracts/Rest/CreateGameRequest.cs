namespace Scrummy.AppContracts.Rest;

public record CreateGameRequest(string Nickname, IEnumerable<Card> Deck);
