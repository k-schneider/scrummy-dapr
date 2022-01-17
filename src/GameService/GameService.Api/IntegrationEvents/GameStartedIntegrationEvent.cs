namespace Scrummy.GameService.Api.IntegrationEvents;

public record GameStartedIntegrationEvent(string GameId) : IntegrationEvent
{
    public const string EventName = "GameStarted";
}
