namespace Scrummy.GameService.Api.IntegrationEvents;

public record VotesResetIntegrationEvent(string GameId) : IntegrationEvent;
