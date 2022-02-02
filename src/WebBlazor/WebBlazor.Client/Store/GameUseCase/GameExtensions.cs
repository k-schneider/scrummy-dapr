public static class GameExtensions
{
    public static bool IsHost(this GameState state)
    {
        return state.Players
            .FirstOrDefault(p => p.PlayerId == state.PlayerId)
            ?.IsHost == true;
    }
}
