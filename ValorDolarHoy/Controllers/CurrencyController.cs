using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValorDolarHoy.Services.Currency;

namespace ValorDolarHoy.Controllers
{
    [ApiController]
    [Route("[controller]")] 
    public class CurrencyController : CustomControllerBase
    {
        private readonly CurrencyService currencyService;

        public CurrencyController(CurrencyService currencyService)
        {
            this.currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestAsync()
        {
            return await base.QueryAsync(this.currencyService.GetLatest());
        }
    }
}