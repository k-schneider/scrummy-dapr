namespace Scrummy.AppContracts;

public record GameSession(string GameId, int PlayerId, string Sid);
// todo: add a timestamp? might be useful so client can clean up old session records
