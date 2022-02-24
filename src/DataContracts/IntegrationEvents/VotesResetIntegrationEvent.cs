namespace Scrummy.IntegrationEvents;

public record VotesResetIntegrationEvent(string GameId) : IntegrationEvent;
