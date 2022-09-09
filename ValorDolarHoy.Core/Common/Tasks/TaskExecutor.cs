using System;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
#pragma warning disable CS1591

namespace ValorDolarHoy.Core.Controllers;

public static class TaskExecutor
{
    public static async Task<IActionResult> ExecuteAsync<T>(IObservable<T> observable)
    {
        return new OkObjectResult(await observable.ToTask());
    }
}