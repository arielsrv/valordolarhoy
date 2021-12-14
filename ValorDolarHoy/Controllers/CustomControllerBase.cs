using Microsoft.AspNetCore.Mvc;
using System;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace ValorDolarHoy.Controllers;

public class CustomControllerBase : ControllerBase
{
    protected async Task<IActionResult> QueryAsync<T>(IObservable<T> observable)
    {
        return this.Ok(await observable.ToTask());
    }
}