using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ValorDolayHoy.Core.Services.Currency;

namespace ValorDolarHoy.Controllers;

[ApiController]
[Route("[controller]")]
public class FallbackController : CustomControllerBase
{
    private readonly CurrencyService currencyService;

    public FallbackController(CurrencyService currencyService)
    {
        this.currencyService = currencyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLatestAsync()
    {
        return await this.QueryAsync(this.currencyService.GetFallback());
    }
}