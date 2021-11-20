using System.Net.Http;
using System.Threading.Tasks;
using ValorDolarHoy.Common;

namespace ValorDolarHoy.Services.Clients
{
    public class BluelyticsClient : Client
    {
        public BluelyticsClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public virtual async Task<BluelyticsResponse> GetLatest()
        {
            const string url = "https://api.bluelytics.com.ar/v2/latest";
            return await this.Get<BluelyticsResponse>(url);
        }
    }
}