namespace Scrummy.GameService.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class GameEventController : ControllerBase
{
    private readonly IActorProxyFactory _actorProxyFactory;
    private readonly IHubContext<GameHub> _hubContext;

    public GameEventController(
        IActorProxyFactory actorProxyFactory,
        IHubContext<GameHub> hubContext)
    {
        _actorProxyFactory = actorProxyFactory;
        _hubContext = hubContext;
    }


    [HttpPost("SessionConnectedEvent")]
    [Topic(Constants.DaprPubSubName, "SessionConnectedEvent")]
    public Task HandleAsync(SessionConnectedEvent integrationEvent)
    {
        return Task.CompletedTask;
    }

    [HttpPost("SessionDisconnectedEvent")]
    [Topic(Constants.DaprPubSubName, "SessionDisconnectedEvent")]
    public Task HandleAsync(SessionDisconnectedEvent integrationEvent)
    {
        return Task.CompletedTask;
    }

    /*
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
    */
}
