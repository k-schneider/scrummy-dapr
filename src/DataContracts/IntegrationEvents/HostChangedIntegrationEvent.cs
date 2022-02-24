namespace Scrummy.IntegrationEvents;

public record HostChangedIntegrationEvent(
    string Sid,
    int PlayerId,
    string GameId) : IntegrationEvent;
