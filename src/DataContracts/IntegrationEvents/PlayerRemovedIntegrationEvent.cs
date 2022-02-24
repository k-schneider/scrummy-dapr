namespace Scrummy.IntegrationEvents;

public record PlayerRemovedIntegrationEvent(
    string Sid,
    int PlayerId,
    string GameId) : IntegrationEvent;
