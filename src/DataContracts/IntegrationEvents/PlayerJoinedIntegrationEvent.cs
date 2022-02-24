namespace Scrummy.IntegrationEvents;

public record PlayerJoinedIntegrationEvent(
    string Sid,
    int PlayerId,
    string Nickname,
    string GameId) : IntegrationEvent;
