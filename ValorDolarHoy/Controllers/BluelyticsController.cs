using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValorDolarHoy.Services;

namespace ValorDolarHoy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Bluelytics : ControllerBase
    {
        private readonly IBluelyticsService bluelyticsService;

        public Bluelytics(IBluelyticsService bluelyticsService)
        {
            this.bluelyticsService = bluelyticsService;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> GetLatest()
        {
            return Ok(await this.bluelyticsService.GetLatest());
        }
    }
}