using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValorDolarHoy.Core.Common.Tasks;
using ValorDolarHoy.Core.Services.Currency;

namespace ValorDolarHoy.Core.Controllers;

/// <summary>
///     Main controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyService currencyService;

    /// <summary>
    ///     Creates the instance
    /// </summary>
    /// <param name="currencyService"></param>
    public CurrencyController(ICurrencyService currencyService)
    {
        this.currencyService = currencyService;
    }

    /// <summary>
    ///     Get latest from bluelytics
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(CurrencyDto), 200)]
    public async Task<IActionResult> GetLatestAsync()
    {
        return await TaskExecutor.ExecuteAsync(this.currencyService.GetLatest());
    }
}