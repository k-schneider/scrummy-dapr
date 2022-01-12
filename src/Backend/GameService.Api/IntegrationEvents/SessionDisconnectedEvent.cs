namespace Scrummy.GameService.Api.IntegrationEvents;

public record SessionDisconnectedEvent(string ConnectionId, string Sid, string GameId) : IntegrationEvent;
