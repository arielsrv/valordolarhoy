using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ValorDolarHoy.Services
{
    public interface IBluelyticsService
    {
        Task<BluelyticsResponse> GetLatest();
    }

    public class BluelyticsResponse
    {
        public Oficial Oficial { get; set; }
        public Blue Blue { get; set; }
    }

    public class Oficial
    {
        public double ValueAvg { get; set; }
        public double ValueSell { get; set; }
        public double ValueBuy { get; set; }
    }

    public class Blue
    {
        public double ValueAvg { get; set; }
        public double ValueSell { get; set; }
        public double ValueBuy { get; set; }
    }

    public class BluelyticsService : IBluelyticsService
    {
        private readonly HttpClient httpClient;

        public BluelyticsService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<BluelyticsResponse> GetLatest()
        {
            HttpResponseMessage httpResponseMessage = await this.httpClient.GetAsync("/v2/latest");

            string response = await httpResponseMessage.Content.ReadAsStringAsync();

            BluelyticsResponse bluelyticsResponse = JsonConvert.DeserializeObject<BluelyticsResponse>(response);

            return bluelyticsResponse;
        }
    }
}