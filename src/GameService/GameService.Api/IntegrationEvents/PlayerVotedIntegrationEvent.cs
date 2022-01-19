namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerVotedIntegrationEvent(
    string Sid,
    int PlayerId,
    string Vote,
    string GameId) : IntegrationEvent;
