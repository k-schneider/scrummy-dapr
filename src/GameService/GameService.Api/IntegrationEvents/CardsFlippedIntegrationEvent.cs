namespace Scrummy.GameService.Api.IntegrationEvents;

public record CardsFlippedIntegrationEvent(
    string GameId,
    Dictionary<int, string> Votes) : IntegrationEvent;
