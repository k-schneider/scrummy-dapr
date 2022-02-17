namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerLeftIntegrationEvent(
    string Sid,
    int PlayerId,
    string GameId,
    bool IsHost,
    int PlayerCount) : IntegrationEvent;
