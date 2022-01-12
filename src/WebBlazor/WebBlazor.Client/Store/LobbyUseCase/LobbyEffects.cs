namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

public class LobbyEffects
{
    private const string NicknameKey = "nickname";

    private readonly ILocalStorageService _localStorage;

    public LobbyEffects(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    [EffectMethod]
    public async Task HandleStoreInitializedAction(StoreInitializedAction _, IDispatcher dispatcher)
    {
        var nickname = await _localStorage.GetItemAsync<string?>(NicknameKey) ?? string.Empty;

        dispatcher.Dispatch(new HydrateLobbyAction(nickname));
    }

    /*
    [EffectMethod]
    public async Task HandleRememberNicknameAction(RememberNicknameAction action, IDispatcher _)
    {
        await _localStorage.SetItemAsync(NicknameKey, action.Nickname);
    }
    */
}
