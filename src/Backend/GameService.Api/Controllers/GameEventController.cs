namespace Scrummy.GameService.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class GameEventController : ControllerBase
{
    private readonly IActorProxyFactory _actorProxyFactory;

    public GameEventController(IActorProxyFactory actorProxyFactory)
    {
        _actorProxyFactory = actorProxyFactory;
    }

    [HttpPost("PlayerDisconnectedFromGame")]
    [Topic(Constants.DaprPubSubName, "PlayerDisconnectedFromGameEvent")]
    public Task HandleAsync(PlayerDisconnectedFromGameEvent integrationEvent)
    {
        return GetGameActor(integrationEvent.GameId).NotifyPlayerDisconnected(integrationEvent.PlayerId);
    }

    [HttpPost("PlayerJoinedGame")]
    [Topic(Constants.DaprPubSubName, "PlayerJoinedGameEvent")]
    public Task HandleAsync(PlayerJoinedGameEvent integrationEvent)
    {
        return GetGameActor(integrationEvent.GameId).NotifyPlayerJoined(integrationEvent.PlayerId, integrationEvent.Nickname);
    }

    [HttpPost("PlayerLeftGame")]
    [Topic(Constants.DaprPubSubName, "PlayerLeftGameEvent")]
    public Task HandleAsync(PlayerLeftGameEvent integrationEvent)
    {
        return GetGameActor(integrationEvent.GameId).NotifyPlayerLeft(integrationEvent.PlayerId);
    }

    private IGameActor GetGameActor(string gameId)
    {
        return _actorProxyFactory.CreateActorProxy<IGameActor>(
            new ActorId(gameId),
            typeof(GameActor).Name);
    }
}
