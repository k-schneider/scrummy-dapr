namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

public class LobbyEffects
{
    private const string NicknameKey = "nickname";
    private const string GamesKey = "games";

    private readonly IAppApi _appApi;
    private readonly IState<LobbyState> _lobbyState;
    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager _navigationManager;

    public LobbyEffects(
        IAppApi appApi,
        IState<LobbyState> lobbyState,
        ILocalStorageService localStorage,
        NavigationManager navigationManager)
    {
        _appApi = appApi;
        _lobbyState = lobbyState;
        _localStorage = localStorage;
        _navigationManager = navigationManager;
    }

    [EffectMethod]
    public async Task HandleStoreInitializedAction(StoreInitializedAction _, IDispatcher dispatcher)
    {
        var nickname = await _localStorage.GetItemAsync<string>(NicknameKey) ?? string.Empty;
        var games = await _localStorage.GetItemAsync<Dictionary<string, GameSession>>(GamesKey) ?? new Dictionary<string, GameSession>();

        dispatcher.Dispatch(new InitializeLobbyAction(nickname, games));
    }

    [EffectMethod]
    public async Task HandleCreateRoomAction(CreateRoomAction action, IDispatcher dispatcher)
    {
        await RememberNickname();

        try
        {
            var gameSession = await _appApi.CreateGame(new CreateGameRequest(action.Nickname));
            dispatcher.Dispatch(new CreateRoomSuccessAction(gameSession));
        }
        catch (Exception exc)
        {
            dispatcher.Dispatch(new CreateRoomFailedAction(exc.Message));
        }
    }

    [EffectMethod]
    public async Task HandleCreateRoomSuccessAction(CreateRoomSuccessAction action, IDispatcher _)
    {
        await RememberGames();
        _navigationManager.NavigateTo($"/room/{action.Game.GameId}");
    }

    [EffectMethod]
    public async Task HandleJoinRoomAction(JoinRoomAction action, IDispatcher dispatcher)
    {
        await RememberNickname();

        try
        {
            var gameSession = await _appApi.JoinGame(action.GameId, new JoinGameRequest(action.Nickname));
            dispatcher.Dispatch(new JoinRoomSuccessAction(gameSession));
        }
        catch (Exception exc)
        {
            dispatcher.Dispatch(new JoinRoomFailedAction(exc.Message));
        }
    }

    [EffectMethod]
    public async Task HandleJoinRoomSuccessAction(JoinRoomSuccessAction action, IDispatcher _)
    {
        await RememberGames();
        _navigationManager.NavigateTo($"/room/{action.Game.GameId}");
    }

    private async Task RememberGames()
    {
        await _localStorage.SetItemAsync(GamesKey, _lobbyState.Value.Games);
    }

    private async Task RememberNickname()
    {
        await _localStorage.SetItemAsync(NicknameKey, _lobbyState.Value.Nickname);
    }
}
