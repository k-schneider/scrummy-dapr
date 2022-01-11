namespace Scrummy.BlazorClient.Store.GameUseCase;

public class GameEffects
{
    private readonly IState<IdentityState> _identityState;
    private readonly NavigationManager _navigationManager;
    private HubConnection? _hubConnection;

    public GameEffects(
        IState<IdentityState> identityState,
        NavigationManager navigationManager
    )
    {
        _identityState = identityState;
        _navigationManager = navigationManager;
    }

    [EffectMethod]
    public async Task HandleJoinGameAction(JoinGameAction action, IDispatcher dispatcher)
    {
        if (_hubConnection == null)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri($"/h/gamehub?sid={_identityState.Value.Sid}"))
                .Build();

            await _hubConnection.StartAsync();
        }

        await _hubConnection.InvokeAsync("JoinGame", action.GameId, _identityState.Value.Nickname);

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
