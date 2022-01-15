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
}
