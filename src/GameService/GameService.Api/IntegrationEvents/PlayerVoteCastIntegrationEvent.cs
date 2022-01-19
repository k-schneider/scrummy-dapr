namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerVoteCastIntegrationEvent(
    string Sid,
    int PlayerId,
    string Vote,
    string? PreviousVote,
    string GameId) : IntegrationEvent;
