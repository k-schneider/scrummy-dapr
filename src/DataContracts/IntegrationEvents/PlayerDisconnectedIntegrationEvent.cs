namespace Scrummy.IntegrationEvents;

public record PlayerDisconnectedIntegrationEvent(
    string ConnectionId,
    string Sid,
    string GameId,
    int PlayerId,
    int ConnectionCount) : IntegrationEvent;