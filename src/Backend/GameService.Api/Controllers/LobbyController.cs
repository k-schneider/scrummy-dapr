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
        string gameId = string.Empty;

        // Find a new game identifier and start the game
        while (gameId == string.Empty)
        {
            var potentialId = GameId.Generate();

            var game = _actorProxyFactory.CreateActorProxy<IGameActor>(
                new ActorId(potentialId),
                typeof(GameActor).Name);

            if (!await game.Exists())
            {
                await game.Start(cancellationToken);
                gameId = potentialId;
            }
        }

        return Ok(gameId);
    }
}
