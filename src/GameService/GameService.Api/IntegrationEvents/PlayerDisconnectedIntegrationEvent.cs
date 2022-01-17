namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerDisconnectedIntegrationEvent(
    string ConnectionId,
    string Sid,
    string GameId,
    int PlayerId,
    int ConnectionCount) : IntegrationEvent
{
    public const string EventName = "PlayerDisconnected";
}
