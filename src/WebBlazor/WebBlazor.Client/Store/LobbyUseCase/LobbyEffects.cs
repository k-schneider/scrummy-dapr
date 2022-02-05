namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

public class LobbyEffects
{
    private const string NicknameKey = "nickname";
    private const string GamesKey = "games";

    private readonly IAppApi _appApi;
    private readonly IState<LobbyState> _lobbyState;
    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager _navigationManager;
    private readonly IToastService _toastService;

    public LobbyEffects(
        IAppApi appApi,
        IState<LobbyState> lobbyState,
        ILocalStorageService localStorage,
        NavigationManager navigationManager,
        IToastService toastService)
    {
        _appApi = appApi;
        _lobbyState = lobbyState;
        _localStorage = localStorage;
        _navigationManager = navigationManager;
        _toastService = toastService;
    }

    [EffectMethod]
    public async Task HandleStoreInitializedAction(StoreInitializedAction _, IDispatcher dispatcher)
    {
        var nickname = await _localStorage.GetItemAsync<string>(NicknameKey) ?? string.Empty;

        // Try and deserialize remembered games, if this fails we'll just ignore it
        Dictionary<string, GameMembership>? games = null;
        try
        {
            games = await _localStorage.GetItemAsync<Dictionary<string, GameMembership>>(GamesKey);
        }
        catch
        {
            Console.WriteLine("Warning: unable to load games from local storage.");
        }

        dispatcher.Dispatch(new InitializeLobbyAction(nickname, games ?? new Dictionary<string, GameMembership>()));
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
        catch (ApiException exc)
        {
            dispatcher.Dispatch(new CreateGameFailedAction(exc.GetError()));
        }
    }

    [EffectMethod]
    public Task HandleCreateGameFailedAction(CreateGameFailedAction action, IDispatcher _)
    {
        _toastService.ShowError($"Unable to create game: [{action.Error}]");
        return Task.CompletedTask;
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
        catch (ApiException exc)
        {
            dispatcher.Dispatch(new JoinGameFailedAction(exc.GetError()));
        }
    }

    [EffectMethod]
    public async Task HandleJoinGameSuccessAction(JoinGameSuccessAction action, IDispatcher _)
    {
        await RememberGames();
        _navigationManager.NavigateTo($"/room/{action.Game.GameId}");
    }

    [EffectMethod]
    public Task HandleJoinGameFailedAction(JoinGameFailedAction action, IDispatcher _)
    {
        _toastService.ShowError($"Unable to join game: [{action.Error}]");
        return Task.CompletedTask;
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
