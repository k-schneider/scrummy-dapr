namespace Scrummy.GameService.Api.IntegrationEvents;

public record NewVoteStartedIntegrationEvent(string GameId) : IntegrationEvent;
