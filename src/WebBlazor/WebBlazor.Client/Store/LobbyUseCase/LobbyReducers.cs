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
    public static LobbyState ReduceCreateRoomSuccessAction(LobbyState state, CreateRoomSuccessAction action)
    {
        var games = new Dictionary<string, GameMembership>()
            .Concat(state.Games)
            .ToDictionary(x => x.Key, x => x.Value);

        games[action.Game.GameId] = action.Game;

        return state with
        {
            CreatingRoom = false,
            Games = games
        };
    }

    [ReducerMethod]
    public static LobbyState ReduceJoinRoomAction(LobbyState state, JoinRoomAction action) =>
        state with
        {
            JoiningRoom = true,
            Nickname = action.Nickname
        };

    [ReducerMethod]
    public static LobbyState ReduceJoinRoomFailedAction(LobbyState state, JoinRoomFailedAction action) =>
        state with
        {
            JoiningRoom = false
        };

    [ReducerMethod]
    public static LobbyState ReduceJoinRoomSuccessAction(LobbyState state, JoinRoomSuccessAction action)
    {
        var games = new Dictionary<string, GameMembership>()
            .Concat(state.Games)
            .ToDictionary(x => x.Key, x => x.Value);

        games[action.Game.GameId] = action.Game;

        return state with
        {
            JoiningRoom = false,
            Games = games
        };
    }

    [ReducerMethod]
    public static LobbyState ReduceLeaveRoomAction(LobbyState state, LeaveRoomAction action) =>
        state with
        {
            LeavingRoom = true
        };

    [ReducerMethod]
    public static LobbyState ReduceLeaveRoomFailedAction(LobbyState state, LeaveRoomFailedAction action) =>
        state with
        {
            LeavingRoom = false
        };

    [ReducerMethod]
    public static LobbyState ReduceLeaveRoomSuccessAction(LobbyState state, LeaveRoomSuccessAction action)
    {
        var games = new Dictionary<string, GameMembership>()
            .Concat(state.Games)
            .ToDictionary(x => x.Key, x => x.Value);

        games.Remove(action.GameId);

        return state with
        {
            LeavingRoom = false,
            Games = games
        };
    }
}
