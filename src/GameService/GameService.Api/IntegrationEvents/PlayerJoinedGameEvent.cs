namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerJoinedGameEvent(string Sid, int PlayerId, string Nickname) : IntegrationEvent;
