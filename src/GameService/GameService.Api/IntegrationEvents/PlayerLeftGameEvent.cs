namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerLeftGameEvent(string Sid, int PlayerId, string GameId) : IntegrationEvent;
