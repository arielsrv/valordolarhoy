using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValorDolarHoy.Core.Common.Tasks;
using ValorDolarHoy.Core.Services.Currency;

namespace ValorDolarHoy.Core.Controllers;

/// <summary>
///     FallbackController
/// </summary>
[ApiController]
[Route("[controller]")]
public class FallbackController : ControllerBase
{
    private readonly ICurrencyService currencyService;

    /// <summary>
    ///     FallbackController
    /// </summary>
    /// <param name="currencyService"></param>
    public FallbackController(ICurrencyService currencyService)
    {
        this.currencyService = currencyService;
    }

    /// <summary>
    ///     Get from fallback
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(CurrencyDto), 200)]
    public async Task<IActionResult> GetLatestAsync()
    {
        return await TaskExecutor.ExecuteAsync(this.currencyService.GetFallback());
    }

    /// <summary>
    ///     Example for zip
    /// </summary>
    /// <returns></returns>
    [HttpGet("test/zip")]
    [ProducesResponseType(typeof(CurrencyDto), 200)]
    public async Task<IActionResult> ZipAsync()
    {
        return await TaskExecutor.ExecuteAsync(this.currencyService.GetAll());
    }
}