namespace Scrummy.GameService.Api.IntegrationEvents;

public record GameEndedIntegrationEvent(string GameId) : IntegrationEvent;
