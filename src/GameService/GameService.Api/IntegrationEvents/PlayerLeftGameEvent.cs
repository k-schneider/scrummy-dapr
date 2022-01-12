namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerLeftGameEvent(string PlayerId, string GameId) : IntegrationEvent;
