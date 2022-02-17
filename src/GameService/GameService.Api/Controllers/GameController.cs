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

    [HttpPost("player/{sid}/vote")]
    public async Task<IActionResult> CastVote(string sid, CastVoteRequest request, CancellationToken cancellationToken)
    {
        await GetPlayerActor(sid).CastVote(request.Vote, cancellationToken);
        return Ok();
    }

    [HttpPost("lobby/create")]
    public async Task<IActionResult> CreateGame(CreateGameRequest request, CancellationToken cancellationToken)
    {
        var gameId = await GetLobbyActor().CreateGame(request.Deck, cancellationToken);

        var sid = NewSid();
        var playerId = await GetPlayerActor(sid).JoinGame(gameId, request.Nickname, cancellationToken);

        return Ok(new CreateGameResponse(gameId, playerId, sid));
    }

    [HttpPost("player/{sid}/flip")]
    public async Task<IActionResult> FlipCards(string sid, CancellationToken cancellationToken)
    {
        await GetPlayerActor(sid).FlipCards(cancellationToken);
        return Ok();
    }

    [HttpGet("game/{gameId}")]
    public async Task<IActionResult> GameExists(string gameId, CancellationToken cancellationToken)
    {
        var exists = await GetLobbyActor().GameExists(gameId, cancellationToken);
        return Ok(exists);
    }

    [HttpPost("game/{gameId}/join")]
    public async Task<IActionResult> JoinGame(string gameId, JoinGameRequest request, CancellationToken cancellationToken)
    {
        if (!await GetLobbyActor().GameExists(gameId))
        {
            return NotFound("Game does not exist");
        }

        var sid = NewSid();
        var playerId = await GetPlayerActor(sid).JoinGame(gameId, request.Nickname, cancellationToken);

        return Ok(new JoinGameResponse(gameId, playerId, sid));
    }

    [HttpPost("player/{sid}/leave")]
    public async Task<IActionResult> LeaveGame(string sid, CancellationToken cancellationToken)
    {
        await GetPlayerActor(sid).LeaveGame(cancellationToken);
        return Ok();
    }

    [HttpPost("player/{sid}/nudge")]
    public async Task<IActionResult> NudgePlayer(string sid, NudgePlayerRequest request, CancellationToken cancellationToken)
    {
        await GetPlayerActor(sid).NudgePlayer(request.PlayerId, cancellationToken);
        return Ok();
    }

    [HttpPost("player/{sid}/next")]
    public async Task<IActionResult> PlayAgain(string sid, CancellationToken cancellationToken)
    {
        await GetPlayerActor(sid).PlayAgain(cancellationToken);
        return Ok();
    }

    [HttpPost("player/{sid}/promote")]
    public async Task<IActionResult> PromotePlayer(string sid, PromotePlayerRequest request, CancellationToken cancellationToken)
    {
        await GetPlayerActor(sid).PromotePlayer(request.PlayerId, cancellationToken);
        return Ok();
    }

    [HttpDelete("player/{sid}/vote")]
    public async Task<IActionResult> RecallVote(string sid, CancellationToken cancellationToken)
    {
        await GetPlayerActor(sid).RecallVote(cancellationToken);
        return Ok();
    }

    [HttpDelete("player/{sid}/player")]
    public async Task<IActionResult> RemovePlayer(string sid, RemovePlayerRequest request, CancellationToken cancellationToken)
    {
        await GetPlayerActor(sid).RemovePlayer(request.PlayerId, cancellationToken);
        return Ok();
    }

    [HttpDelete("player/{sid}/votes")]
    public async Task<IActionResult> ResetVotes(string sid, CancellationToken cancellationToken)
    {
        await GetPlayerActor(sid).ResetVotes(cancellationToken);
        return Ok();
    }

    [HttpPost("player/{sid}/spectating")]
    public async Task<IActionResult> StartSpectating(string sid, CancellationToken cancellationToken)
    {
        await GetPlayerActor(sid).StartSpectating(cancellationToken);
        return Ok();
    }

    [HttpDelete("player/{sid}/spectating")]
    public async Task<IActionResult> StopSpectating(string sid, CancellationToken cancellationToken)
    {
        await GetPlayerActor(sid).StopSpectating(cancellationToken);
        return Ok();
    }

    [HttpPut("player/{sid}/nickname")]
    public async Task<IActionResult> UpdateNickname(string sid, UpdateNicknameRequest request, CancellationToken cancellationToken)
    {
        await GetPlayerActor(sid).UpdateNickname(request.Nickname, cancellationToken);
        return Ok();
    }

    private ILobbyActor GetLobbyActor() =>
        _actorProxyFactory.CreateActorProxy<ILobbyActor>(
            new ActorId(Guid.Empty.ToString()),
            typeof(LobbyActor).Name);

    private IPlayerActor GetPlayerActor(string sid) =>
        _actorProxyFactory.CreateActorProxy<IPlayerActor>(
            new ActorId(sid),
            typeof(PlayerActor).Name);

    private string NewSid() => Guid.NewGuid().ToString();
}
