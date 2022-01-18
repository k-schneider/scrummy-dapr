namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

public static class GameReducers
{
    [ReducerMethod]
    public static GameState ReduceConnectToGameAction(GameState state, ConnectToGameAction _) =>
        state with
        {
            Connecting = true
        };

    [ReducerMethod]
    public static GameState ReduceConnectToGameSuccessAction(GameState state, ConnectToGameSuccessAction _) =>
        state with
        {
            Connecting = false
        };

    [ReducerMethod]
    public static GameState ReduceConnectToGameFailedAction(GameState state, ConnectToGameFailedAction _) =>
        state with
        {
            Connecting = false
        };

    [ReducerMethod]
    public static GameState ReduceDisconnectFromGameAction(GameState state, DisconnectFromGameAction _) =>
        state with
        {
            Disconnecting = true
        };

    [ReducerMethod]
    public static GameState ReduceDisconnectFromGameSuccessAction(GameState state, DisconnectFromGameSuccessAction _) =>
        state with
        {
            Disconnecting = false
        };

    [ReducerMethod]
    public static GameState ReduceDisconnectFromGameFailedAction(GameState state, DisconnectFromGameFailedAction _) =>
        state with
        {
            Disconnecting = false
        };

    [ReducerMethod]
    public static GameState ReduceLeaveGameAction(GameState state, LeaveGameAction action) =>
        state with
        {
            Leaving = true
        };

    [ReducerMethod]
    public static GameState ReduceLeaveGameFailedAction(GameState state, LeaveGameFailedAction action) =>
        state with
        {
            Leaving = false
        };

    [ReducerMethod]
    public static GameState ReduceLeaveGameSuccessAction(GameState state, LeaveGameSuccessAction action) =>
        state with
        {
            Leaving = false
        };

    [ReducerMethod]
    public static GameState ReducePlayerConnectedAction(GameState state, PlayerConnectedAction action)
    {
        var player = state.Players
            .Where(p => p.PlayerId == action.PlayerId)
            .First() with
            {
                IsConnected = true
            };

        return state with
        {
            Players = state.Players
                .Where(p => p.PlayerId != action.PlayerId)
                .Append(player)
                .ToArray()
        };
    }

    [ReducerMethod]
    public static GameState ReducePlayerDisconnectedAction(GameState state, PlayerDisconnectedAction action)
    {
        var player = state.Players
            .Where(p => p.PlayerId == action.PlayerId)
            .First() with
            {
                IsConnected = false
            };

        return state with
        {
            Players = state.Players
                .Where(p => p.PlayerId != action.PlayerId)
                .Append(player)
                .ToArray()
        };
    }

    [ReducerMethod]
    public static GameState ReducePlayerJoinedGameAction(GameState state, PlayerJoinedGameAction action) =>
        state with
        {
            Players = state.Players
                .Append(new Player(
                    action.PlayerId,
                    action.Nickname,
                    false))
                .ToArray()
        };

    [ReducerMethod]
    public static GameState ReducePlayerLeftGameAction(GameState state, PlayerLeftGameAction action) =>
        state with
        {
            Players = state.Players
                .Where(p => p.PlayerId != action.PlayerId)
                .ToArray()
        };

    [ReducerMethod]
    public static GameState ReduceSyncGameAction(GameState state, SyncGameAction action) =>
        state with
        {
            Players = action.Snapshot.Players.Select(p => new Player(p.PlayerId, p.Nickname, p.IsConnected)),
        };
}
