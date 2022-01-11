namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerDisconnectedFromGameEvent(string PlayerId, string GameId) : IntegrationEvent;
