using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using ValorDolarHoy.Common;

namespace ValorDolarHoy.Clients.Currency
{
    public interface ICurrencyClient
    {
        IObservable<CurrencyResponse> Get();
    }

    public class CurrencyClient : Client, ICurrencyClient
    {
        public CurrencyClient(HttpClient httpClient, ILogger<CurrencyClient> logger) : base(httpClient, logger)
        {
        }

        public IObservable<CurrencyResponse> Get()
        {
            const string url = "https://api.bluelytics.com.ar/v2/latest";
            return this.Get<CurrencyResponse>(url);
        }
    }
}