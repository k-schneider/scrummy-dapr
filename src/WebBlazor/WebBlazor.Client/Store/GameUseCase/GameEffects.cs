namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

public class GameEffects
{
    private readonly IAppApi _appApi;
    private readonly IState<GameState> _gameState;
    private readonly HttpClient _httpClient;
    private readonly IState<LobbyState> _lobbyState;
    private readonly NavigationManager _navigationManager;
    private HubConnection? _hubConnection;

    public GameEffects(
        IAppApi appApi,
        IState<GameState> gameState,
        HttpClient httpClient,
        IState<LobbyState> lobbyState,
        NavigationManager navigationManager)
    {
        _appApi = appApi;
        _gameState = gameState;
        _httpClient = httpClient;
        _lobbyState = lobbyState;
        _navigationManager = navigationManager;
    }

    [EffectMethod]
    public async Task HandleCastVoteAction(CastVoteAction action, IDispatcher dispatcher)
    {
        try
        {
            await _appApi.CastVote(_gameState.Value.GameId, new CastVoteRequest(_gameState.Value.Sid, action.Vote));
            dispatcher.Dispatch(new CastVoteSuccessAction());
        }
        catch (Exception exc)
        {
            dispatcher.Dispatch(new CastVoteFailedAction(exc.Message));
        }
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

            if (_hubConnection is null)
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_navigationManager.ToAbsoluteUri($"/h/gamehub?sid={game.Sid}"))
                    .Build();

                _hubConnection.On<SyncGameMessage>(GameHubMethods.SyncGame, message =>
                    dispatcher.Dispatch(new SyncGameAction(message.Snapshot)));

                _hubConnection.On<PlayerConnectedMessage>(GameHubMethods.PlayerConnected, message =>
                    dispatcher.Dispatch(new PlayerConnectedAction(message.PlayerId)));

                _hubConnection.On<PlayerDisconnectedMessage>(GameHubMethods.PlayerDisconnected, message =>
                    dispatcher.Dispatch(new PlayerDisconnectedAction(message.PlayerId)));

                _hubConnection.On<PlayerJoinedGameMessage>(GameHubMethods.PlayerJoinedGame, message =>
                    dispatcher.Dispatch(new PlayerJoinedGameAction(message.PlayerId, message.Nickname)));

                _hubConnection.On<PlayerLeftGameMessage>(GameHubMethods.PlayerLeftGame, message =>
                    dispatcher.Dispatch(new PlayerLeftGameAction(message.PlayerId)));

                _hubConnection.On<PlayerVotedMessage>(GameHubMethods.PlayerVoted, message =>
                    dispatcher.Dispatch(new PlayerVotedAction(message.PlayerId)));

                _hubConnection.On<VoteRecordedMessage>(GameHubMethods.VoteRecorded, message =>
                    dispatcher.Dispatch(new VoteRecordedAction(message.Vote)));

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
    public async Task HandleLeaveGameAction(LeaveGameAction action, IDispatcher dispatcher)
    {
        try
        {
            var game = _lobbyState.Value.Games[action.GameId];
            await _appApi.LeaveGame(action.GameId, new LeaveGameRequest(game.Sid));
            dispatcher.Dispatch(new LeaveGameSuccessAction(game.GameId));
        }
        catch (Exception exc)
        {
            dispatcher.Dispatch(new LeaveGameFailedAction(exc.Message));
        }
    }

    [EffectMethod]
    public Task HandleLeaveGameSuccessAction(LeaveGameSuccessAction action, IDispatcher dispatcher)
    {
        _navigationManager.NavigateTo($"/");
        dispatcher.Dispatch(new ForgetGameAction(action.GameId));
        return Task.CompletedTask;
    }

    [EffectMethod]
    public Task HandlePlayerLeftGameAction(PlayerLeftGameAction action, IDispatcher dispatcher)
    {
        if (action.PlayerId == _gameState.Value.PlayerId)
        {
            // Player left on another tab so redirect this tab too
            _navigationManager.NavigateTo($"/");
        }
        return Task.CompletedTask;
    }
}
