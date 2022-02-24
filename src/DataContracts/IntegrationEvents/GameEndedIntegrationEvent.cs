namespace Scrummy.IntegrationEvents;

public record GameEndedIntegrationEvent(string GameId) : IntegrationEvent;
