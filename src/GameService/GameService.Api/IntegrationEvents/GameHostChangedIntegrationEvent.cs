namespace Scrummy.GameService.Api.IntegrationEvents;

public record GameHostChangedIntegrationEvent(
    string PreviousHostSid,
    int PreviousHostPlayerId,
    string PreviousHostNickname,
    string NewHostSid,
    int NewHostPlayerId,
    string NewHostNickname,
    string GameId) : IntegrationEvent;
