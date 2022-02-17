namespace Scrummy.GameService.Api.IntegrationEvents;

public record HostChangedIntegrationEvent(
    string Sid,
    int PlayerId,
    string GameId) : IntegrationEvent;
