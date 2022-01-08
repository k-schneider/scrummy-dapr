namespace Scrummy.BlazorClient.Store.IdentityUseCase;

public static class IdentityReducers
{
    [ReducerMethod]
    public static IdentityState ReduceHydrateIdentityAction(IdentityState state, HydrateIdentityAction action) =>
        state with
        {
            Sid = action.Sid,
            Nickname = action.Nickname
        };

    [ReducerMethod]
    public static IdentityState ReduceRememberNicknameAction(IdentityState state, RememberNicknameAction action) =>
        state with
        {
            Nickname = action.Nickname
        };
}
