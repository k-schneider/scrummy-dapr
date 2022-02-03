namespace Scrummy.GameService.Api.IntegrationEvents;

public record PlayerNicknameChangedIntegrationEvent(
    string Sid,
    int PlayerId,
    string Nickname,
    string GameId) : IntegrationEvent;
