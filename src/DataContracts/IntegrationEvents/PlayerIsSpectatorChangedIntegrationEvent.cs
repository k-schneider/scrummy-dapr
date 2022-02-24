namespace Scrummy.IntegrationEvents;

public record PlayerIsSpectatorChangedIntegrationEvent(
    string Sid,
    int PlayerId,
    bool IsSpectator,
    string GameId) : IntegrationEvent;
