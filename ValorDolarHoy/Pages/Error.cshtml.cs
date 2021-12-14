// using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
// using System.Diagnostics;

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

    // public void OnGet()
    // {
    //     this.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;
    // }
}