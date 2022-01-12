namespace GameService.Api.Controllers;

[Route("api")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IActorProxyFactory _actorProxyFactory;

    public GameController(IActorProxyFactory actorProxyFactory)
    {
        _actorProxyFactory = actorProxyFactory;
    }

    [HttpPost("lobby/create")]
    public async Task<IActionResult> CreateGame(string nickname, CancellationToken cancellationToken)
    {
        var gameId = await GetLobbyActor().CreateGame(cancellationToken);
        var (sid, playerId) = await GetGameActor(gameId).AddPlayer(nickname, cancellationToken);

        return Ok(new
        {
            GameId = gameId,
            PlayerId = playerId,
            Sid = sid
        });
    }

    [HttpPost("game/{gameId}/join")]
    public async Task<IActionResult> JoinGame(string gameId, string nickname, CancellationToken cancellationToken)
    {
        var (sid, playerId) = await GetGameActor(gameId).AddPlayer(nickname, cancellationToken);

        return Ok(new
        {
            GameId = gameId,
            PlayerId = playerId,
            Sid = sid
        });
    }

    [HttpPost("game/{gameId}/leave")]
    public async Task<IActionResult> LeaveGame(string gameId, string sid, CancellationToken cancellationToken)
    {
        await GetGameActor(gameId).RemovePlayer(sid, cancellationToken);
        return Ok();
    }

    private IGameActor GetGameActor(string gameId)
    {
        return _actorProxyFactory.CreateActorProxy<IGameActor>(
            new ActorId(gameId),
            typeof(GameActor).Name);
    }

    private ILobbyActor GetLobbyActor()
    {
        return _actorProxyFactory.CreateActorProxy<ILobbyActor>(
            new ActorId(Guid.Empty.ToString()),
            typeof(LobbyActor).Name);
    }
}
