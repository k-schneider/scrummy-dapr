namespace Scrummy.GameService.Api.IntegrationEvents;

public record HostChangedIntegrationEvent(
    string PreviousHostSid,
    int PreviousHostPlayerId,
    string PreviousHostNickname,
    string NewHostSid,
    int NewHostPlayerId,
    string NewHostNickname,
    string GameId) : IntegrationEvent;
