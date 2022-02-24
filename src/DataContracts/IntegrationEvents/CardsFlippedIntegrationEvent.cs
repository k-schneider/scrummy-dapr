namespace Scrummy.IntegrationEvents;

public record CardsFlippedIntegrationEvent(string GameId) : IntegrationEvent;
