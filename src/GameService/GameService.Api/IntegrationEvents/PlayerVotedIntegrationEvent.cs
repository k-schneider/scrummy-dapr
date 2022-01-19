namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerVotedIntegrationEvent(
    string Sid,
    int PlayerId,
    string Vote,
    string? PreviousVote,
    string GameId) : IntegrationEvent;
