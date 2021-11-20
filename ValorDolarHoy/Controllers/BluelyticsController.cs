using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValorDolarHoy.Services;

namespace ValorDolarHoy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Bluelytics : ControllerBase
    {
        private readonly BluelyticsService bluelyticsService;

        public Bluelytics(BluelyticsService bluelyticsService)
        {
            this.bluelyticsService = bluelyticsService;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> GetLatestAsync()
        {
            return Ok(await this.bluelyticsService.GetLatestAsync());
        }
    }
}