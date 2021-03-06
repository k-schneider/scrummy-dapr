namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

public class GameEffects
{
    private const string DisableNudgeAnimationKey = "disableNudgeAnimation";
    private const string MuteSoundsKey = "muteSounds";

    private readonly IAppApi _appApi;
    private readonly IState<GameState> _gameState;
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly IState<LobbyState> _lobbyState;
    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager _navigationManager;
    private readonly IToastService _toastService;
    private HubConnection? _hubConnection;
    private Timer? _nudgeTimer;

    public GameEffects(
        IAppApi appApi,
        IState<GameState> gameState,
        HttpClient httpClient,
        IJSRuntime jsRuntime,
        IState<LobbyState> lobbyState,
        ILocalStorageService localStorage,
        NavigationManager navigationManager,
        IToastService toastService)
    {
        _appApi = appApi;
        _gameState = gameState;
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _lobbyState = lobbyState;
        _localStorage = localStorage;
        _navigationManager = navigationManager;
        _toastService = toastService;
    }

    [EffectMethod]
    public async Task HandleStoreInitializedAction(StoreInitializedAction _, IDispatcher dispatcher)
    {
        if (await _localStorage.GetItemAsync<bool>(DisableNudgeAnimationKey))
        {
            dispatcher.Dispatch(new DisableNudgeAnimationAction());
        }

        if (await _localStorage.GetItemAsync<bool>(MuteSoundsKey))
        {
            dispatcher.Dispatch(new MuteSoundsAction());
        }
    }

    [EffectMethod]
    public async Task HandleCastVoteAction(CastVoteAction action, IDispatcher dispatcher)
    {
        try
        {
            await _appApi.CastVote(
                _gameState.Value.Sid!,
                new CastVoteRequest(action.Vote));

            dispatcher.Dispatch(new CastVoteSuccessAction());
        }
        catch (ApiException exc)
        {
            dispatcher.Dispatch(new CastVoteFailedAction(exc.GetError()));
        }
    }

    [EffectMethod]
    public Task HandleCastVoteFailedAction(CastVoteFailedAction action, IDispatcher _)
    {
        _toastService.ShowError($"Unable to cast vote: [{action.Error}]");
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleConnectToGameAction(ConnectToGameAction action, IDispatcher dispatcher)
    {
        try
        {
            var game = _lobbyState.Value.Games[action.GameId];

            if (game is null)
            {
                throw new Exception("Must join game prior to connecting to it.");
            }

            if (!await _appApi.GameExists(game.GameId))
            {
                throw new Exception("Game does not exist.");
            }

            if (_hubConnection is null)
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_navigationManager.ToAbsoluteUri(
                        $"/h/gamehub?sid={game.Sid}"),
                        options => options.Transports = HttpTransportType.WebSockets)
                    .Build();

                _hubConnection.On<CardsFlippedMessage>(GameHubMethods.CardsFlipped, message =>
                    dispatcher.Dispatch(new CardsFlippedAction(message.Votes)));

                _hubConnection.On(GameHubMethods.GameEnded, () =>
                    dispatcher.Dispatch(new GameEndedAction()));

                _hubConnection.On<HostChangedMessage>(GameHubMethods.HostChanged, message =>
                    dispatcher.Dispatch(new HostChangedAction(message.PlayerId)));

                _hubConnection.On(GameHubMethods.NewVoteStarted, () =>
                    dispatcher.Dispatch(new NewVoteStartedAction()));

                _hubConnection.On<PlayerConnectedMessage>(GameHubMethods.PlayerConnected, message =>
                    dispatcher.Dispatch(new PlayerConnectedAction(message.PlayerId)));

                _hubConnection.On<PlayerDisconnectedMessage>(GameHubMethods.PlayerDisconnected, message =>
                    dispatcher.Dispatch(new PlayerDisconnectedAction(message.PlayerId)));

                _hubConnection.On<PlayerJoinedMessage>(GameHubMethods.PlayerJoined, message =>
                    dispatcher.Dispatch(new PlayerJoinedAction(message.PlayerId, message.Nickname)));

                _hubConnection.On<PlayerLeftMessage>(GameHubMethods.PlayerLeft, message =>
                    dispatcher.Dispatch(new PlayerLeftAction(message.PlayerId)));

                _hubConnection.On<PlayerNicknameChangedMessage>(GameHubMethods.PlayerNicknameChanged, message =>
                    dispatcher.Dispatch(new PlayerNicknameChangedAction(message.PlayerId, message.Nickname)));

                _hubConnection.On<PlayerNudgedMessage>(GameHubMethods.PlayerNudged, message =>
                    dispatcher.Dispatch(new PlayerNudgedAction(message.FromPlayerId, message.ToPlayerId)));

                _hubConnection.On<PlayerRemovedMessage>(GameHubMethods.PlayerRemoved, message =>
                    dispatcher.Dispatch(new PlayerRemovedAction(message.PlayerId)));

                _hubConnection.On<PlayerIsSpectatorChangedMessage>(GameHubMethods.PlayerIsSpectatorChanged, message =>
                    dispatcher.Dispatch(new PlayerIsSpectatorChangedAction(message.PlayerId, message.IsSpectator)));

                _hubConnection.On<PlayerVoteCastMessage>(GameHubMethods.PlayerVoteCast, message =>
                    dispatcher.Dispatch(new PlayerVoteCastAction(message.PlayerId, message.HadPreviousVote, message.Vote)));

                _hubConnection.On<PlayerVoteRecalledMessage>(GameHubMethods.PlayerVoteRecalled, message =>
                    dispatcher.Dispatch(new PlayerVoteRecalledAction(message.PlayerId)));

                _hubConnection.On<ReceiveGameStateMessage>(GameHubMethods.ReceiveGameState, message =>
                    dispatcher.Dispatch(new ReceiveGameStateAction(message.Game)));

                _hubConnection.On(GameHubMethods.VotesReset, () =>
                    dispatcher.Dispatch(new VotesResetAction()));

                _hubConnection.Closed += exc =>
                {
                    if (_gameState.Value.Connected)
                    {
                        dispatcher.Dispatch(new GameConnectionClosedAction());
                    }

                    return Task.CompletedTask;
                };

                await _hubConnection.StartAsync();
            }

            dispatcher.Dispatch(new ConnectToGameSuccessAction(game.Sid, game.PlayerId));
        }
        catch (Exception exc)
        {
            dispatcher.Dispatch(new ConnectToGameFailedAction(exc.Message));
        }
    }

    [EffectMethod]
    public Task HandleConnectToGameFailedAction(ConnectToGameFailedAction action, IDispatcher dispatcher)
    {
        _toastService.ShowError($"Unable to connect to game: [{action.Error}]");
        dispatcher.Dispatch(new ForgetGameAction(_gameState.Value.GameId!));
        _navigationManager.NavigateTo("/");
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleCopyLinkToClipboardAction(CopyLinkToClipboardAction _, IDispatcher dispatcher)
    {
        await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", _navigationManager.Uri);
        dispatcher.Dispatch(new CloseInvitePopoverAction());
        _toastService.ShowSuccess("Link copied to clipboard.");
    }

    [EffectMethod]
    public async Task HandleDisableNudgeAnimationAction(DisableNudgeAnimationAction _, IDispatcher _1)
    {
        await _localStorage.SetItemAsync<bool>(DisableNudgeAnimationKey, true);
    }

    [EffectMethod]
    public async Task HandleDisconnectFromGameAction(DisconnectFromGameAction _, IDispatcher dispatcher)
    {
        try
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }

            dispatcher.Dispatch(new DisconnectFromGameSuccessAction());
        }
        catch (Exception exc)
        {
            dispatcher.Dispatch(new DisconnectFromGameFailedAction(exc.Message));
        }
    }

    [EffectMethod]
    public async Task HandleEnableNudgeAnimationAction(EnableNudgeAnimationAction _, IDispatcher _1)
    {
        await _localStorage.RemoveItemAsync(DisableNudgeAnimationKey);
    }

    [EffectMethod]
    public async Task HandleFlipCardsAction(FlipCardsAction action, IDispatcher dispatcher)
    {
        try
        {
            await _appApi.FlipCards(_gameState.Value.Sid!);

            dispatcher.Dispatch(new FlipCardsSuccessAction());
        }
        catch (ApiException exc)
        {
            dispatcher.Dispatch(new FlipCardsFailedAction(exc.GetError()));
        }
    }

    [EffectMethod]
    public Task HandleFlipCardsFailedAction(FlipCardsFailedAction action, IDispatcher _)
    {
        _toastService.ShowError($"Unable to flip cards: [{action.Error}]");
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandleGameConnectionClosedAction(GameConnectionClosedAction _, IDispatcher dispatcher)
    {
        if (_gameState.Value.GamePhase is null)
        {
            // If the connection is closed before receiving game state
            // then the connection was rejected and we should forget this game
            dispatcher.Dispatch(new ForgetGameAction(_gameState.Value.GameId!));
            _toastService.ShowError("Connection rejected by the server.");
            _navigationManager.NavigateTo("/");
        }

        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandleGameEndedAction(GameEndedAction _, IDispatcher dispatcher)
    {
        if (!_gameState.Value.Leaving && !_gameState.Value.Disconnecting)
        {
            dispatcher.Dispatch(new ForgetGameAction(_gameState.Value.GameId!));
            _navigationManager.NavigateTo($"/");
            _toastService.ShowWarning("Game has ended.");
        }
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleLeaveGameAction(LeaveGameAction action, IDispatcher dispatcher)
    {
        try
        {
            await _appApi.LeaveGame(_gameState.Value.Sid!);
            dispatcher.Dispatch(new LeaveGameSuccessAction());
        }
        catch (ApiException exc)
        {
            dispatcher.Dispatch(new LeaveGameFailedAction(exc.GetError()));
        }
    }

    [EffectMethod]
    public Task HandleLeaveGameFailedAction(LeaveGameFailedAction action, IDispatcher _)
    {
        _toastService.ShowError($"Failed to leave game: [{action.Error}]");
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandleLeaveGameSuccessAction(LeaveGameSuccessAction _, IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new ForgetGameAction(_gameState.Value.GameId!));
        _navigationManager.NavigateTo($"/");
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleMuteSoundsAction(MuteSoundsAction _, IDispatcher _1)
    {
        await _localStorage.SetItemAsync<bool>(MuteSoundsKey, true);
    }

    [EffectMethod]
    public async Task HandleNudgePlayerAction(NudgePlayerAction action, IDispatcher dispatcher)
    {
        try
        {
            await _appApi.NudgePlayer(
                _gameState.Value.Sid!,
                new NudgePlayerRequest(action.PlayerId));

            dispatcher.Dispatch(new NudgePlayerSuccessAction());
        }
        catch (ApiException exc)
        {
            dispatcher.Dispatch(new NudgePlayerFailedAction(exc.GetError()));
        }
    }

    [EffectMethod]
    public Task HandleNudgePlayerFailedAction(NudgePlayerFailedAction action, IDispatcher _)
    {
        _toastService.ShowError($"Failed to nudge player: [{action.Error}]");
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandleNudgePlayerSuccessAction(NudgePlayerSuccessAction _, IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new CloseOtherPlayerMenuAction());
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandlePlayAgainAction(PlayAgainAction action, IDispatcher dispatcher)
    {
        try
        {
            await _appApi.PlayAgain(_gameState.Value.Sid!);

            dispatcher.Dispatch(new PlayAgainSuccessAction());
        }
        catch (ApiException exc)
        {
            dispatcher.Dispatch(new PlayAgainFailedAction(exc.GetError()));
        }
    }

    [EffectMethod]
    public Task HandlePlayAgainFailedAction(PlayAgainFailedAction action, IDispatcher _)
    {
        _toastService.ShowError($"Unable to play again: [{action.Error}]");
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandlePlayerLeftAction(PlayerLeftAction action, IDispatcher dispatcher)
    {
        if (action.PlayerId == _gameState.Value.PlayerId
            && !_gameState.Value.Leaving
            && !_gameState.Value.Disconnecting)
        {
            dispatcher.Dispatch(new ForgetGameAction(_gameState.Value.GameId!));
            _navigationManager.NavigateTo($"/");
        }
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandlePlayerNudgedAction(PlayerNudgedAction action, IDispatcher dispatcher)
    {
        if (action.ToPlayerId == _gameState.Value.PlayerId)
        {
            if (_nudgeTimer != null)
            {
                await _nudgeTimer.DisposeAsync();
            }

            _nudgeTimer = new Timer(_ => dispatcher.Dispatch(new ResetNudgedAction()), null, 1500, Timeout.Infinite);
        }
    }

    [EffectMethod]
    public Task HandlePlayerRemovedAction(PlayerRemovedAction action, IDispatcher dispatcher)
    {
        if (action.PlayerId == _gameState.Value.PlayerId)
        {
            dispatcher.Dispatch(new ForgetGameAction(_gameState.Value.GameId!));
            _navigationManager.NavigateTo($"/");
        }
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandlePromotePlayerAction(PromotePlayerAction action, IDispatcher dispatcher)
    {
        try
        {
            await _appApi.PromotePlayer(
                _gameState.Value.Sid!,
                new PromotePlayerRequest(action.PlayerId));

            dispatcher.Dispatch(new PromotePlayerSuccessAction());
        }
        catch (ApiException exc)
        {
            dispatcher.Dispatch(new PromotePlayerFailedAction(exc.GetError()));
        }
    }

    [EffectMethod]
    public Task HandlePromotePlayerFailedAction(PromotePlayerFailedAction action, IDispatcher _)
    {
        _toastService.ShowError($"Unable to promote player: [{action.Error}]");
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandlePromotePlayerSuccessAction(PromotePlayerSuccessAction _, IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new CloseOtherPlayerMenuAction());
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleRecallVoteAction(RecallVoteAction action, IDispatcher dispatcher)
    {
        try
        {
            await _appApi.RecallVote(_gameState.Value.Sid!);

            dispatcher.Dispatch(new RecallVoteSuccessAction());
        }
        catch (ApiException exc)
        {
            dispatcher.Dispatch(new RecallVoteFailedAction(exc.GetError()));
        }
    }

    [EffectMethod]
    public Task HandleRecallVoteFailedAction(RecallVoteFailedAction action, IDispatcher _)
    {
        _toastService.ShowError($"Unable to recall vote: [{action.Error}]");
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandleReconnectToGameAction(ReconnectToGameAction _, IDispatcher _1)
    {
        _navigationManager.NavigateTo(_navigationManager.Uri, true);
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleRemovePlayerAction(RemovePlayerAction action, IDispatcher dispatcher)
    {
        try
        {
            await _appApi.RemovePlayer(
                _gameState.Value.Sid!,
                new RemovePlayerRequest(action.PlayerId));

            dispatcher.Dispatch(new RemovePlayerSuccessAction());
        }
        catch (ApiException exc)
        {
            dispatcher.Dispatch(new RemovePlayerFailedAction(exc.GetError()));
        }
    }

    [EffectMethod]
    public Task HandleRemovePlayerFailedAction(RemovePlayerFailedAction action, IDispatcher _)
    {
        _toastService.ShowError($"Unable to remove player: [{action.Error}]");
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandleRemovePlayerSuccessAction(RemovePlayerSuccessAction _, IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new CloseOtherPlayerMenuAction());
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleResetVotesAction(ResetVotesAction action, IDispatcher dispatcher)
    {
        try
        {
            await _appApi.ResetVotes(_gameState.Value.Sid!);

            dispatcher.Dispatch(new ResetVotesSuccessAction());
        }
        catch (ApiException exc)
        {
            dispatcher.Dispatch(new ResetVotesFailedAction(exc.GetError()));
        }
    }

    [EffectMethod]
    public Task HandleResetVotesFailedAction(ResetVotesFailedAction action, IDispatcher _)
    {
        _toastService.ShowError($"Unable to reset votes: [{action.Error}]");
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleUpdateNicknameAction(UpdateNicknameAction action, IDispatcher dispatcher)
    {
        try
        {
            await _appApi.UpdateNickname(
                _gameState.Value.Sid!,
                new UpdateNicknameRequest(action.Nickname));

            dispatcher.Dispatch(new UpdateNicknameSuccessAction());
        }
        catch (ApiException exc)
        {
            dispatcher.Dispatch(new UpdateNicknameFailedAction(exc.GetError()));
        }
    }

    [EffectMethod]
    public Task HandleUpdateNicknameFailedAction(UpdateNicknameFailedAction action, IDispatcher _)
    {
        _toastService.ShowError($"Unable to update nickname: [{action.Error}]");
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandleUpdateNicknameSuccessAction(UpdateNicknameSuccessAction _, IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new ClosePlayerPopoverAction());
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleStartSpectatingAction(StartSpectatingAction action, IDispatcher dispatcher)
    {
        try
        {
            await _appApi.StartSpectating(_gameState.Value.Sid!);
            dispatcher.Dispatch(new StartSpectatingSuccessAction());
        }
        catch (ApiException exc)
        {
            dispatcher.Dispatch(new StartSpectatingFailedAction(exc.GetError()));
        }
    }

    [EffectMethod]
    public Task HandleStartSpectatingFailedAction(StartSpectatingFailedAction action, IDispatcher _)
    {
        _toastService.ShowError($"Unable to start spectating: [{action.Error}]");
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleStopSpectatingAction(StopSpectatingAction action, IDispatcher dispatcher)
    {
        try
        {
            await _appApi.StopSpectating(_gameState.Value.Sid!);
            dispatcher.Dispatch(new StopSpectatingSuccessAction());
        }
        catch (ApiException exc)
        {
            dispatcher.Dispatch(new StopSpectatingFailedAction(exc.GetError()));
        }
    }

    [EffectMethod]
    public Task HandleStopSpectatingFailedAction(StopSpectatingFailedAction action, IDispatcher _)
    {
        _toastService.ShowError($"Unable to stop spectating: [{action.Error}]");
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleUnmuteSoundsAction(UnmuteSoundsAction _, IDispatcher _1)
    {
        await _localStorage.RemoveItemAsync(MuteSoundsKey);
    }
}
