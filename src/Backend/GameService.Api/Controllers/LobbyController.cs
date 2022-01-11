namespace GameService.Api.Controllers;

[Route("api/lobby")]
[ApiController]
public class LobbyController : ControllerBase
{
    private readonly IActorProxyFactory _actorProxyFactory;

    public LobbyController(IActorProxyFactory actorProxyFactory)
    {
        _actorProxyFactory = actorProxyFactory;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateGame(CancellationToken cancellationToken)
    {
        var lobby = _actorProxyFactory.CreateActorProxy<ILobbyActor>(
            new ActorId(Guid.Empty.ToString()),
            typeof(LobbyActor).Name);

        var gameId = await lobby.CreateGame(cancellationToken);

        return Ok(gameId);
    }
}
