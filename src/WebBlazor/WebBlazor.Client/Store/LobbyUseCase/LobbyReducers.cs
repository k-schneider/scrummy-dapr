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
    public static LobbyState ReduceCreateGameAction(LobbyState state, CreateGameAction action) =>
        state with
        {
            CreatingGame = true,
            Nickname = action.Nickname
        };

    [ReducerMethod]
    public static LobbyState ReduceCreateGameFailedAction(LobbyState state, CreateGameFailedAction action) =>
        state with
        {
            CreatingGame = false
        };

    [ReducerMethod]
    public static LobbyState ReduceCreateGameSuccessAction(LobbyState state, CreateGameSuccessAction action)
    {
        var games = new Dictionary<string, GameMembership>()
            .Concat(state.Games)
            .ToDictionary(x => x.Key, x => x.Value);

        games[action.Game.GameId] = action.Game;

        return state with
        {
            CreatingGame = false,
            Games = games
        };
    }

    [ReducerMethod]
    public static LobbyState ReduceForgetGameAction(LobbyState state, ForgetGameAction action)
    {
        var games = new Dictionary<string, GameMembership>()
            .Concat(state.Games)
            .ToDictionary(x => x.Key, x => x.Value);

        games.Remove(action.GameId);

        return state with
        {
            Games = games
        };
    }

    [ReducerMethod]
    public static LobbyState ReduceJoinGameAction(LobbyState state, JoinGameAction action) =>
        state with
        {
            JoiningGame = true,
            Nickname = action.Nickname
        };

    [ReducerMethod]
    public static LobbyState ReduceJoinGameFailedAction(LobbyState state, JoinGameFailedAction action) =>
        state with
        {
            JoiningGame = false
        };

    [ReducerMethod]
    public static LobbyState ReduceJoinGameSuccessAction(LobbyState state, JoinGameSuccessAction action)
    {
        var games = new Dictionary<string, GameMembership>()
            .Concat(state.Games)
            .ToDictionary(x => x.Key, x => x.Value);

        games[action.Game.GameId] = action.Game;

        return state with
        {
            JoiningGame = false,
            Games = games
        };
    }
}
