using System.Net.Http;
using System.Threading.Tasks;
using ValorDolarHoy.Common;

namespace ValorDolarHoy.Services.Clients
{
    public interface IBluelyticsClient
    {
        Task<BluelyticsResponse> GetLatestAsync();
    }

    public class BluelyticsClient : Client, IBluelyticsClient
    {
        public BluelyticsClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<BluelyticsResponse> GetLatestAsync()
        {
            const string url = "https://api.bluelytics.com.ar/v2/latest";
            return await this.GetAsync<BluelyticsResponse>(url);
        }
    }
}