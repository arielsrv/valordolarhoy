using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValorDolarHoy.Controllers;
using ValorDolarHoy.Core.Services.Currency;

namespace ValorDolarHoy.Core.Controllers;

[ApiController]
[Route("[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly CurrencyService currencyService;

    public CurrencyController(CurrencyService currencyService)
    {
        this.currencyService = currencyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLatestAsync()
    {
        return await TaskExecutor.ExecuteAsync(this.currencyService.GetLatest());
    }
}