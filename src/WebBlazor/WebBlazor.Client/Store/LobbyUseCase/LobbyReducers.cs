namespace Scrummy.WebBlazor.Client.Store.LobbyUseCase;

public static class LobbyReducers
{
    [ReducerMethod]
    public static LobbyState ReduceInitializeLobbyAction(LobbyState state, InitializeLobbyAction action) =>
        state with
        {
            Games = action.Games,
            Initialized = true,
            Nickname = action.Nickname
        };

    [ReducerMethod]
    public static LobbyState ReduceCreateRoomAction(LobbyState state, CreateRoomAction action) =>
        state with
        {
            CreatingRoom = true,
            Nickname = action.Nickname
        };

    [ReducerMethod]
    public static LobbyState ReduceCreateRoomFailedAction(LobbyState state, CreateRoomFailedAction action) =>
        state with
        {
            CreatingRoom = false
        };

    [ReducerMethod]
    public static LobbyState ReduceCreateRoomSuccessAction(LobbyState state, CreateRoomSuccessAction action) =>
        state with
        {
            CreatingRoom = false,
            Games = state.Games.Append(action.Game)
        };
}
