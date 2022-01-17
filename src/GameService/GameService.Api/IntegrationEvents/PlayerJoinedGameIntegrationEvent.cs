namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerJoinedGameIntegrationEvent(
    string Sid,
    int PlayerId,
    string Nickname) : IntegrationEvent;