using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValorDolarHoy.Core.Services.Currency;

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

    [HttpGet("test/zip")]
    public async Task<IActionResult> ZipAsync()
    {
        return await this.QueryAsync(this.currencyService.GetAll());
    }
}