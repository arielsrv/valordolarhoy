using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValorDolarHoy.Controllers;
using ValorDolarHoy.Core.Services.Currency;

namespace ValorDolarHoy.Core.Controllers;

[ApiController]
[Route("[controller]")]
public class FallbackController : ControllerBase
{
    private readonly ICurrencyService currencyService;

    public FallbackController(ICurrencyService currencyService)
    {
        this.currencyService = currencyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetLatestAsync()
    {
        return await TaskExecutor.ExecuteAsync(this.currencyService.GetFallback());
    }

    [HttpGet("test/zip")]
    public async Task<IActionResult> ZipAsync()
    {
        return await TaskExecutor.ExecuteAsync(this.currencyService.GetAll());
    }
}