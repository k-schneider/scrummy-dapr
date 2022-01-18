namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerLeftGameIntegrationEvent(
    string Sid,
    int PlayerId,
    string GameId) : IntegrationEvent;
