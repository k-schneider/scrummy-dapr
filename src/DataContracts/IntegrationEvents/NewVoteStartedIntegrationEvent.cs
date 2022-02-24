namespace Scrummy.IntegrationEvents;

public record NewVoteStartedIntegrationEvent(string GameId) : IntegrationEvent;
