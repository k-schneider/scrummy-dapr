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
        var games = await _localStorage.GetItemAsync<Dictionary<string, GameMembership>>(GamesKey) ?? new Dictionary<string, GameMembership>();

        dispatcher.Dispatch(new InitializeLobbyAction(nickname, games));
    }

    [EffectMethod]
    public async Task HandleCreateGameAction(CreateGameAction action, IDispatcher dispatcher)
    {
        await RememberNickname();

        try
        {
            var response = await _appApi.CreateGame(new CreateGameRequest(action.Nickname));
            var gameMembership = new GameMembership(response.GameId, response.PlayerId, response.Sid);
            dispatcher.Dispatch(new CreateGameSuccessAction(gameMembership));
        }
        catch (Exception exc)
        {
            dispatcher.Dispatch(new CreateGameFailedAction(exc.Message));
        }
    }

    [EffectMethod]
    public async Task HandleCreateGameSuccessAction(CreateGameSuccessAction action, IDispatcher _)
    {
        await RememberGames();
        _navigationManager.NavigateTo($"/room/{action.Game.GameId}");
    }

    [EffectMethod]
    public async Task HandleJoinGameAction(JoinGameAction action, IDispatcher dispatcher)
    {
        await RememberNickname();

        try
        {
            var response = await _appApi.JoinGame(action.GameId, new JoinGameRequest(action.Nickname));
            var game = new GameMembership(response.GameId, response.PlayerId, response.Sid);
            dispatcher.Dispatch(new JoinGameSuccessAction(game));
        }
        catch (Exception exc)
        {
            dispatcher.Dispatch(new JoinGameFailedAction(exc.Message));
        }
    }

    [EffectMethod]
    public async Task HandleJoinGameSuccessAction(JoinGameSuccessAction action, IDispatcher _)
    {
        await RememberGames();
        _navigationManager.NavigateTo($"/room/{action.Game.GameId}");
    }

    [EffectMethod]
    public async Task HandleForgetGameAction(ForgetGameAction action, IDispatcher _)
    {
        await RememberGames();
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
