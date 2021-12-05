using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValorDolarHoy.Services;

namespace ValorDolarHoy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FallbackController : CustomControllerBase
    {
        private readonly BluelyticsService bluelyticsService;

        public FallbackController(BluelyticsService bluelyticsService)
        {
            this.bluelyticsService = bluelyticsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestAsync()
        {
            return await base.QueryAsync(this.bluelyticsService.GetFallback());
        }
    }
}