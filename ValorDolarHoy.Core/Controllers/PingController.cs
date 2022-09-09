using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ValorDolarHoy.Core.Common.Extensions;

namespace ValorDolarHoy.Core.Controllers;

[ApiController]
[Route("[controller]")]
public class PingController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<string> Pong()
    {
        return WarmupExecutor.Initialized
            ? this.StatusCode(StatusCodes.Status200OK, "pong")
            : this.StatusCode(StatusCodes.Status503ServiceUnavailable, "offline");
    }
}