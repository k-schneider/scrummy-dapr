namespace Scrummy.IntegrationEvents;

public record PlayerLeftIntegrationEvent(
    string Sid,
    int PlayerId,
    string GameId,
    bool IsHost,
    int PlayerCount) : IntegrationEvent;
