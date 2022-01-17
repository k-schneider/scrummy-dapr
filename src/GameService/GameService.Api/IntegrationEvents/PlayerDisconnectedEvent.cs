namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerDisconnectedEvent(string ConnectionId, string Sid, string GameId, int PlayerId, int ConnectionCount) : IntegrationEvent;
