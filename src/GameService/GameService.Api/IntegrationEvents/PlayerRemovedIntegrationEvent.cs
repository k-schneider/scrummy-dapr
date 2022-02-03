namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerRemovedIntegrationEvent(
    string Sid,
    int PlayerId,
    string GameId) : IntegrationEvent;
