using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValorDolarHoy.Services;

namespace ValorDolarHoy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly BluelyticsService bluelyticsService;

        public CurrencyController(BluelyticsService bluelyticsService)
        {
            this.bluelyticsService = bluelyticsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestAsync()
        {
            return Ok(await this.bluelyticsService
                .GetLatest()
                .ToTask());
        }
    }
}