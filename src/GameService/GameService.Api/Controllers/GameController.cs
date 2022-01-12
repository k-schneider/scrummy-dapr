namespace Scrummy.GameService.Api.Controllers;

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
    [ProducesResponseType(typeof(SessionDetail), 200)]
    public async Task<IActionResult> CreateGame(CreateGameRequest request, CancellationToken cancellationToken)
    {
        var gameId = await GetLobbyActor().CreateGame(cancellationToken);
        var (sid, playerId) = await GetGameActor(gameId).AddPlayer(request.Nickname, cancellationToken);

        return Ok(new SessionDetail
        {
            GameId = gameId,
            PlayerId = playerId,
            Sid = sid
        });
    }

    [HttpPost("game/{gameId}/join")]
    [ProducesResponseType(typeof(SessionDetail), 200)]
    public async Task<IActionResult> JoinGame(string gameId, JoinGameRequest request, CancellationToken cancellationToken)
    {
        var (sid, playerId) = await GetGameActor(gameId).AddPlayer(request.Nickname, cancellationToken);

        return Ok(new SessionDetail
        {
            GameId = gameId,
            PlayerId = playerId,
            Sid = sid
        });
    }

    [HttpPost("game/{gameId}/leave")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> LeaveGame(string gameId, LeaveGameRequest request, CancellationToken cancellationToken)
    {
        await GetGameActor(gameId).RemovePlayer(request.Sid, cancellationToken);
        return Ok();
    }

    private IGameActor GetGameActor(string gameId) =>
        _actorProxyFactory.CreateActorProxy<IGameActor>(
            new ActorId(gameId),
            typeof(GameActor).Name);

    private ILobbyActor GetLobbyActor() =>
        _actorProxyFactory.CreateActorProxy<ILobbyActor>(
            new ActorId(Guid.Empty.ToString()),
            typeof(LobbyActor).Name);
}
