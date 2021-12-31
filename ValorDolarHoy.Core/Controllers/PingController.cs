using Microsoft.AspNetCore.Mvc;

namespace ValorDolarHoy.Core.Controllers;

[ApiController]
[Route("[controller]")]
public class PingController : ControllerBase
{
    [HttpGet]
    public string Pong()
    {
        return "pong";
    }
}