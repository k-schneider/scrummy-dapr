namespace Scrummy.IntegrationEvents;

public record PlayerVoteRecalledIntegrationEvent(
    string Sid,
    int PlayerId,
    string? PreviousVote,
    string GameId) : IntegrationEvent;
