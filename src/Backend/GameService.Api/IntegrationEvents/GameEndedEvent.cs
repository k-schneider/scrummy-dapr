namespace Scrummy.GameService.Api.IntegrationEvents;

public record GameEndedEvent(string GameId) : IntegrationEvent;
