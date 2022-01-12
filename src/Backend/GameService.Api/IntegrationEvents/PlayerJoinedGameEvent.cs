namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerJoinedGameEvent(string PlayerId, string GameId) : IntegrationEvent;
