namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

public record CardsFlippedAction(Dictionary<int, string> Votes);

public record CastVoteAction(string Vote);
public record CastVoteFailedAction(string Error);
public record CastVoteSuccessAction();

public record CloseInvitePopoverAction();
public record CloseOtherPlayerMenuAction();
public record ClosePlayerPopoverAction();

public record ConnectToGameAction(string GameId);
public record ConnectToGameFailedAction(string Error);
public record ConnectToGameSuccessAction(string Sid, int PlayerId);

public record CopyLinkToClipboardAction();

public record DisconnectFromGameAction();
public record DisconnectFromGameFailedAction(string Error);
public record DisconnectFromGameSuccessAction();

public record FlipCardsAction();
public record FlipCardsFailedAction(string Error);
public record FlipCardsSuccessAction();

public record GameConnectionClosedAction();

public record GameEndedAction();

public record HostChangedAction(int PlayerId);

public record LeaveGameAction();
public record LeaveGameFailedAction(string Error);
public record LeaveGameSuccessAction();

public record NewVoteStartedAction();

public record NudgePlayerAction(int PlayerId);
public record NudgePlayerFailedAction(string Error);
public record NudgePlayerSuccessAction();

public record OpenInvitePopoverAction();
public record OpenOtherPlayerMenuAction(int PlayerId);
public record OpenPlayerPopoverAction();

public record PlayAgainAction();
public record PlayAgainSuccessAction();
public record PlayAgainFailedAction(string Error);

public record PlayerConnectedAction(int PlayerId);
public record PlayerDisconnectedAction(int PlayerId);

public record PlayerIsSpectatorChangedAction(int PlayerId, bool IsSpectator);

public record PlayerJoinedAction(int PlayerId, string Nickname);

public record PlayerLeftAction(int PlayerId);

public record PlayerNicknameChangedAction(int PlayerId, string Nickname);

public record PlayerNudgedAction(int FromPlayerId, int ToPlayerId);

public record PlayerRemovedAction(int PlayerId);

public record PlayerVoteCastAction(int PlayerId, bool hadPreviousVote, string? Vote);
public record PlayerVoteRecalledAction(int PlayerId);

public record PromotePlayerAction(int PlayerId);
public record PromotePlayerFailedAction(string Error);
public record PromotePlayerSuccessAction();

public record RecallVoteAction();
public record RecallVoteFailedAction(string Error);
public record RecallVoteSuccessAction();

public record ReceiveGameStateAction(Game Game);

public record ReconnectToGameAction();

public record RemovePlayerAction(int PlayerId);
public record RemovePlayerFailedAction(string Error);
public record RemovePlayerSuccessAction();

public record ResetNudgedAction();

public record ResetVotesAction();
public record ResetVotesFailedAction(string Error);
public record ResetVotesSuccessAction();

public record UpdateMuteNudgesAction(bool MuteNudges);

public record UpdateNicknameAction(string Nickname);
public record UpdateNicknameFailedAction(string Error);
public record UpdateNicknameSuccessAction();

public record StartSpectatingAction();
public record StartSpectatingFailedAction(string Error);
public record StartSpectatingSuccessAction();

public record StopSpectatingAction();
public record StopSpectatingFailedAction(string Error);
public record StopSpectatingSuccessAction();

public record VotesResetAction();
