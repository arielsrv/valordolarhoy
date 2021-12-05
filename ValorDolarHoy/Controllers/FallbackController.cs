using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValorDolarHoy.Services;

namespace ValorDolarHoy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FallbackController : CustomControllerBase
    {
        private readonly CurrencyService _currencyService;

        public FallbackController(CurrencyService currencyService)
        {
            this._currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestAsync()
        {
            return await base.QueryAsync(this._currencyService.GetFallback());
        }
    }
}