namespace Scrummy.GameService.Api.IntegrationEvents;

public record GameStartedEvent(string GameId) : IntegrationEvent;
