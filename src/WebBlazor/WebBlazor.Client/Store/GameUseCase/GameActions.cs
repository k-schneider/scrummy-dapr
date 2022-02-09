namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

public record CardsFlippedAction(Dictionary<int, string> Votes);

public record CastVoteAction(string Vote);
public record CastVoteSuccessAction();
public record CastVoteFailedAction(string Error);

public record CloseInvitePopoverAction();
public record CloseOtherPlayerMenuAction();
public record ClosePlayerPopoverAction();

public record ConnectToGameAction(string GameId);
public record ConnectToGameSuccessAction(string Sid, int PlayerId);
public record ConnectToGameFailedAction(string Error);

public record CopyLinkToClipboardAction();

public record DisconnectFromGameAction();
public record DisconnectFromGameSuccessAction();
public record DisconnectFromGameFailedAction(string Error);

public record FlipCardsAction();
public record FlipCardsSuccessAction();
public record FlipCardsFailedAction(string Error);

public record GameConnectionClosedAction();

public record GameEndedAction();

public record HostChangedAction(int PlayerId);

public record LeaveGameAction();
public record LeaveGameSuccessAction();
public record LeaveGameFailedAction(string Error);

public record NewVoteStartedAction();

public record NudgePlayerAction(int PlayerId);
public record NudgePlayerSuccessAction();
public record NudgePlayerFailedAction(string Error);

public record OpenInvitePopoverAction();
public record OpenOtherPlayerMenuAction(int PlayerId);
public record OpenPlayerPopoverAction();

public record PlayAgainAction();
public record PlayAgainSuccessAction();
public record PlayAgainFailedAction(string Error);

public record PlayerConnectedAction(int PlayerId);
public record PlayerDisconnectedAction(int PlayerId);

public record PlayerJoinedAction(int PlayerId, string Nickname);

public record PlayerLeftAction(int PlayerId);

public record PlayerNicknameChangedAction(int PlayerId, string Nickname);

public record PlayerNudgedAction(int FromPlayerId, int ToPlayerId);

public record PlayerRemovedAction(int PlayerId);

public record PlayerVoteCastAction(int PlayerId, bool hadPreviousVote, string? Vote);
public record PlayerVoteRecalledAction(int PlayerId);

public record PromotePlayerAction(int PlayerId);
public record PromotePlayerSuccessAction();
public record PromotePlayerFailedAction(string Error);

public record RecallVoteAction();
public record RecallVoteSuccessAction();
public record RecallVoteFailedAction(string Error);

public record ReceiveGameStateAction(Game Game);

public record ReconnectToGameAction();

public record RemovePlayerAction(int PlayerId);
public record RemovePlayerSuccessAction();
public record RemovePlayerFailedAction(string Error);

public record ResetNudgedAction();

public record ResetVotesAction();
public record ResetVotesSuccessAction();
public record ResetVotesFailedAction(string Error);

public record UpdateNicknameAction(string Nickname);
public record UpdateNicknameSuccessAction();
public record UpdateNicknameFailedAction(string Error);

public record VotesResetAction();