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

    [HttpPost("game/{gameId}/vote")]
    public async Task<IActionResult> CastVote(string gameId, CastVoteRequest request, CancellationToken cancellationToken)
    {
        await GetGameActor(gameId).CastVote(request.Sid, request.Vote, cancellationToken);
        return Ok();
    }

    [HttpPost("lobby/create")]
    public async Task<IActionResult> CreateGame(CreateGameRequest request, CancellationToken cancellationToken)
    {
        var gameId = await GetLobbyActor().CreateGame(cancellationToken);
        var (sid, playerId) = await GetGameActor(gameId).AddPlayer(request.Nickname, cancellationToken);

        return Ok(new CreateGameResponse(gameId, playerId, sid));
    }

    [HttpPost("game/{gameId}/flip")]
    public async Task<IActionResult> FlipCards(string gameId, FlipCardsRequest request, CancellationToken cancellationToken)
    {
        await GetGameActor(gameId).FlipCards(request.Sid, cancellationToken);
        return Ok();
    }

    [HttpPost("game/{gameId}/join")]
    public async Task<IActionResult> JoinGame(string gameId, JoinGameRequest request, CancellationToken cancellationToken)
    {
        if (!await GetLobbyActor().GameExists(gameId))
        {
            return NotFound("Game does not exist");
        }

        var (sid, playerId) = await GetGameActor(gameId).AddPlayer(request.Nickname, cancellationToken);

        return Ok(new JoinGameResponse(gameId, playerId, sid));
    }

    [HttpPost("game/{gameId}/leave")]
    public async Task<IActionResult> LeaveGame(string gameId, LeaveGameRequest request, CancellationToken cancellationToken)
    {
        await GetGameActor(gameId).LeaveGame(request.Sid, cancellationToken);
        return Ok();
    }

    [HttpPost("game/{gameId}/nudge")]
    public async Task<IActionResult> NudgePlayer(string gameId, NudgePlayerRequest request, CancellationToken cancellationToken)
    {
        await GetGameActor(gameId).NudgePlayer(request.Sid, request.PlayerId, cancellationToken);
        return Ok();
    }

    [HttpPost("game/{gameId}/next")]
    public async Task<IActionResult> PlayAgain(string gameId, PlayAgainRequest request, CancellationToken cancellationToken)
    {
        await GetGameActor(gameId).PlayAgain(request.Sid, cancellationToken);
        return Ok();
    }

    [HttpPost("game/{gameId}/promote")]
    public async Task<IActionResult> PromotePlayer(string gameId, PromotePlayerRequest request, CancellationToken cancellationToken)
    {
        await GetGameActor(gameId).PromotePlayer(request.Sid, request.PlayerId, cancellationToken);
        return Ok();
    }

    [HttpDelete("game/{gameId}/vote")]
    public async Task<IActionResult> RecallVote(string gameId, RecallVoteRequest request, CancellationToken cancellationToken)
    {
        await GetGameActor(gameId).RecallVote(request.Sid, cancellationToken);
        return Ok();
    }

    [HttpDelete("game/{gameId}/player")]
    public async Task<IActionResult> RemovePlayer(string gameId, RemovePlayerRequest request, CancellationToken cancellationToken)
    {
        await GetGameActor(gameId).RemovePlayer(request.Sid, request.PlayerId, cancellationToken);
        return Ok();
    }

    [HttpDelete("game/{gameId}/votes")]
    public async Task<IActionResult> ResetVotes(string gameId, ResetVotesRequest request, CancellationToken cancellationToken)
    {
        await GetGameActor(gameId).ResetVotes(request.Sid, cancellationToken);
        return Ok();
    }

    [HttpPut("game/{gameId}/nickname")]
    public async Task<IActionResult> UpdateNickname(string gameId, UpdateNicknameRequest request, CancellationToken cancellationToken)
    {
        await GetGameActor(gameId).UpdateNickname(request.Sid, request.Nickname, cancellationToken);
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
