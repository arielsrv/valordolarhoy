using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using ValorDolarHoy.Core.Common.Http;

#pragma warning disable CS1591

namespace ValorDolarHoy.Core.Clients.Currency;

public interface ICurrencyClient
{
    IObservable<CurrencyResponse> Get();
}

public class CurrencyClient(HttpClient httpClient, ILogger<CurrencyClient> logger)
    : Client(httpClient, logger), ICurrencyClient
{
    public IObservable<CurrencyResponse> Get()
    {
        const string url = "https://api.bluelytics.com.ar/v2/latest";
        return this.Get<CurrencyResponse>(url);
    }
}
