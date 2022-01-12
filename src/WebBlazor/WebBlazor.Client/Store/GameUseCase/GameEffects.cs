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
    public async Task HandleJoinGameAction(JoinGameAction action, IDispatcher dispatcher)
    {
        /*
        var session = _lobbyState.Value.Sessions.Where(g => g.GameId == action.GameId).FirstOrDefault();

        if (session is null)
        {
            var response = await _httpClient.PostAsJsonAsync("/g/lobby/join", new { Nickname = _lobbyState.Value.Nickname });
            response.EnsureSuccessStatusCode();
            session = await JsonSerializer.DeserializeAsync<Session>(await response.Content.ReadAsStreamAsync());
            // todo: persist session in localstorage
        }

        if (_hubConnection is null)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri($"/h/gamehub?sid={session!.Sid}"))
                .Build();

            await _hubConnection.StartAsync();
        }
        */

        dispatcher.Dispatch(new JoinGameSuccessAction());
    }

    [EffectMethod]
    public async Task HandleLeaveGameAction(LeaveGameAction _, IDispatcher dispatcher)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }

        dispatcher.Dispatch(new LeaveGameSuccessAction());
    }
}
