namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

public class GameEffects
{
    private readonly HttpClient _httpClient;
    private readonly IState<LobbyState> _lobbyState;
    private readonly NavigationManager _navigationManager;
    private HubConnection? _hubConnection;

    public GameEffects(
        HttpClient httpClient,
        IState<LobbyState> lobbyState,
        NavigationManager navigationManager)
    {
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
}
