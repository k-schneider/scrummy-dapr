namespace Scrummy.WebBlazor.Client.Store.GameUseCase;

public static class GameReducers
{
    [ReducerMethod]
    public static GameState ReduceJoinGameAction(GameState state, JoinGameAction _) =>
        state with
        {
            Joining = true
        };

    [ReducerMethod]
    public static GameState ReduceJoinGameSuccessAction(GameState state, JoinGameSuccessAction _) =>
        state with
        {
            Joining = false
        };

    [ReducerMethod]
    public static GameState ReduceLeaveGameAction(GameState state, LeaveGameAction _) =>
        state with
        {
            Leaving = true
        };

    [ReducerMethod]
    public static GameState ReduceLeaveGameSuccessAction(GameState state, LeaveGameSuccessAction _) =>
        state with
        {
            Leaving = false
        };
}
