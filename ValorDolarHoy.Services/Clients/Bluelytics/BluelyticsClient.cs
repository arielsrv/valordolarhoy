using System;
using System.Net.Http;
using ValorDolarHoy.Common;

namespace ValorDolarHoy.Services.Clients
{
    public interface IBluelyticsClient
    {
        IObservable<BluelyticsResponse> Get();
    }

    public class BluelyticsClient : Client, IBluelyticsClient
    {
        public BluelyticsClient(HttpClient httpClient) : base(httpClient)
        {
        }
        
        public IObservable<BluelyticsResponse> Get()
        {
            const string url = "https://api.bluelytics.com.ar/v2/latest";
            return this.Get<BluelyticsResponse>(url);
        }
    }
}