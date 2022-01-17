namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerConnectedEvent(string ConnectionId, string Sid, string GameId, int PlayerId, int ConnectionCount) : IntegrationEvent;
