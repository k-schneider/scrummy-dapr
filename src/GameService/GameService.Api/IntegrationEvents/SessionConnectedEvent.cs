namespace Scrummy.GameService.Api.IntegrationEvents;

public record SessionConnectedEvent(string ConnectionId, string Sid, string GameId) : IntegrationEvent;
