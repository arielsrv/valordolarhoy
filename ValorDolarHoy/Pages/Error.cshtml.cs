using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ValorDolarHoy.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorModel : PageModel
{
    public ErrorModel(ILogger<ErrorModel> logger)
    {
        this.RequestId = string.Empty;
    }

    public string RequestId { get; }

    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
}