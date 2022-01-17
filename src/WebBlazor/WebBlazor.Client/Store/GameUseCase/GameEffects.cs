namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

public class GameEffects
{
    private readonly IAppApi _appApi;
    private readonly HttpClient _httpClient;
    private readonly IState<LobbyState> _lobbyState;
    private readonly NavigationManager _navigationManager;
    private HubConnection? _hubConnection;

    public GameEffects(
        IAppApi appApi,
        HttpClient httpClient,
        IState<LobbyState> lobbyState,
        NavigationManager navigationManager)
    {
        _appApi = appApi;
        _httpClient = httpClient;
        _lobbyState = lobbyState;
        _navigationManager = navigationManager;
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

                /* todo:
                _hubConnection.On<PlayerConnectedMessage>(GameHubMethods.PlayerConnected, message =>
                    dispatcher.Dispatch(new PlayerConnectedAction(message.PlayerId)));

                _hubConnection.On<PlayerDisconnectedMessage>(GameHubMethods.PlayerDisconnected, message =>
                    dispatcher.Dispatch(new PlayerDisconnectedAction(message.PlayerId)));
                */

                await _hubConnection.StartAsync();
            }

            dispatcher.Dispatch(new ConnectToGameSuccessAction());
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
}
