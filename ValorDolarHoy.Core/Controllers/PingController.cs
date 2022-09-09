using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ValorDolarHoy.Core.Common.Extensions;

namespace ValorDolarHoy.Core.Controllers;

/// <summary>
///     Ping
/// </summary>
[ApiController]
[Route("[controller]")]
public class PingController : ControllerBase
{
    /// <summary>
    ///     Check if app is online
    /// </summary>
    /// <returns>Http Status Code and string message</returns>
    /// <response code="200">ping is available</response>
    /// <response code="503">offline is not available yet</response>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public ActionResult<string> Pong()
    {
        return WarmupExecutor.Initialized
            ? this.StatusCode(StatusCodes.Status200OK, "pong")
            : this.StatusCode(StatusCodes.Status503ServiceUnavailable, "offline");
    }
}