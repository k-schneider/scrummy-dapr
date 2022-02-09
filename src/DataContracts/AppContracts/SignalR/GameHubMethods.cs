namespace Scrummy.AppContracts.SignalR;

public static class GameHubMethods
{
    public const string CardsFlipped = "CardsFlipped";
    public const string GameEnded = "GameEnded";
    public const string HostChanged = "HostChanged";
    public const string NewVoteStarted = "NewVoteStarted";
    public const string PlayerConnected = "PlayerConnected";
    public const string PlayerDisconnected = "PlayerDisconnected";
    public const string PlayerJoined = "PlayerJoined";
    public const string PlayerLeft = "PlayerLeft";
    public const string PlayerNicknameChanged = "PlayerNicknameChanged";
    public const string PlayerNudged = "PlayerNudged";
    public const string PlayerRemoved = "PlayerRemoved";
    public const string PlayerVoteCast = "PlayerVoteCast";
    public const string PlayerVoteRecalled = "PlayerVoteRecalled";
    public const string ReceiveGameState = "ReceiveGameState";
    public const string VotesReset = "VotesReset";
}
