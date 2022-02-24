namespace Scrummy.IntegrationEvents;

public record PlayerNudgedIntegrationEvent(
    string FromSid,
    int FromPlayerId,
    string ToSid,
    int ToPlayerId,
    string GameId) : IntegrationEvent;
