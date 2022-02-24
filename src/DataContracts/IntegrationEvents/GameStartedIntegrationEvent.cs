namespace Scrummy.IntegrationEvents;

public record GameStartedIntegrationEvent(string GameId) : IntegrationEvent;