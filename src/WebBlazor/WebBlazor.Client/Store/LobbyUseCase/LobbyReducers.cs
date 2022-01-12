namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

public static class LobbyReducers
{
    [ReducerMethod]
    public static LobbyState ReduceHydrateLobbyAction(LobbyState state, HydrateLobbyAction action) =>
        state with
        {
            Nickname = action.Nickname
        };

    /*
    [ReducerMethod]
    public static LobbyState ReduceRememberNicknameAction(LobbyState state, RememberNicknameAction action) =>
        state with
        {
            Nickname = action.Nickname
        };
    */
}
