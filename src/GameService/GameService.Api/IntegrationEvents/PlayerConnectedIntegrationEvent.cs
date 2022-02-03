namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerConnectedIntegrationEvent(
    string ConnectionId,
    string Sid,
    string GameId,
    int PlayerId,
    int ConnectionCount) : IntegrationEvent;
