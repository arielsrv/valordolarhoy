using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ValorDolarHoy.Pages;

/// <summary>
///     Error model
/// </summary>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorModel : PageModel
{
    /// <summary>
    ///     ErrorModel
    /// </summary>
    /// <param name="logger"></param>
    public ErrorModel(ILogger<ErrorModel> logger)
    {
        this.RequestId = string.Empty;
    }

    /// <summary>
    ///     RequestId
    /// </summary>
    public string RequestId { get; }

    /// <summary>
    ///     ShowRequestId
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
}