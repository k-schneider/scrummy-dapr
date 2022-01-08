namespace Scrummy.BlazorClient.Store.IdentityUseCase;

public class IdentityEffects
{
    private const string NicknameKey = "nickname";
    private const string SidKey = "sid";

    private readonly ILocalStorageService _localStorage;

    public IdentityEffects(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    [EffectMethod]
    public async Task HandleStoreInitializedAction(StoreInitializedAction _, IDispatcher dispatcher)
    {
        var sid = await _localStorage.GetItemAsync<string?>(SidKey);
        var nickname = await _localStorage.GetItemAsync<string?>(NicknameKey) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(sid))
        {
            sid = Guid.NewGuid().ToString();
            await _localStorage.SetItemAsync(SidKey, sid);
        }

        dispatcher.Dispatch(new HydrateIdentityAction(sid, nickname));
    }

    [EffectMethod]
    public async Task HandleRememberNicknameAction(RememberNicknameAction action, IDispatcher _)
    {
        await _localStorage.SetItemAsync(NicknameKey, action.Nickname);
    }
}
