namespace Scrummy.BlazorClient.Store.GameUseCase;

public static class GameReducers
{
    [ReducerMethod]
    public static GameState ReduceJoinGameAction(GameState state, JoinGameAction _) =>
        state with
        {
            Joining = true
        };

    [ReducerMethod]
    public static GameState ReduceLeaveGameAction(GameState state, LeaveGameAction _) =>
        state with
        {
            Leaving = true
        };
}
