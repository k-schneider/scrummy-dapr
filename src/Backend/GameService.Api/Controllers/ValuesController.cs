using Microsoft.AspNetCore.Mvc;

namespace GameService.Api.Controllers;
[Route("api/test")]
[ApiController]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello World");
    }
}
