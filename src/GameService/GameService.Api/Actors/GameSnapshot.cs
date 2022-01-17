namespace Scrummy.GameService.Api.Actors;

public record GameSnapshot(IEnumerable<PlayerSnapshot> Players);
public record PlayerSnapshot(int PlayerId, string Nickname, bool IsConnected);
