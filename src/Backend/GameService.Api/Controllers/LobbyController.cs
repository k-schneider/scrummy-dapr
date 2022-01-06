using Microsoft.AspNetCore.Mvc;

namespace GameService.Api.Controllers;
[Route("api/lobby")]
[ApiController]
public class LobbyController : ControllerBase
{
    [HttpPost("create")]
    public IActionResult CreateGame()
    {
        return Ok("game-id-goes-here");
    }
}
