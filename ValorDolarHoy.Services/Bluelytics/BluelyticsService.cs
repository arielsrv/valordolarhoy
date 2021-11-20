using System;
using System.Threading.Tasks;
using Polly;
using Polly.Bulkhead;
using ValorDolarHoy.Services.Clients;
using ValorDolarHoy.Common.Caching;

namespace ValorDolarHoy.Services
{
    public class BluelyticsService : IBluelyticsService
    {
        private readonly IBluelyticsClient bluelyticsClient;

        private readonly Cache<string, BluelyticsDto> appCache = CacheBuilder<string, BluelyticsDto>
            .NewBuilder()
            .Size(4)
            .ExpireAfterWrite(TimeSpan.FromMinutes(5))
            .Build();

        private readonly AsyncBulkheadPolicy bulkheadPolicy = Policy.BulkheadAsync(10);

        public BluelyticsService(IBluelyticsClient bluelyticsClient)
        {
            this.bluelyticsClient = bluelyticsClient;
        }

        public async Task<BluelyticsDto> GetLatest()
        {
            string cacheKey = GetCacheKey();

            BluelyticsDto bluelyticsDto = this.appCache.GetIfPresent(cacheKey);

            if (bluelyticsDto != null)
            {
                return bluelyticsDto;
            }

            bluelyticsDto = await GetFromApi();
            
            await bulkheadPolicy.ExecuteAsync(() =>
            {
                return Task.Run(() => this.appCache.Put(cacheKey, bluelyticsDto));
            });
            
            return bluelyticsDto;
        }

        private async Task<BluelyticsDto> GetFromApi()
        {
            BluelyticsResponse bluelyticsResponse = await this.bluelyticsClient.GetLatest();

            BluelyticsDto bluelyticsDto = new()
            {
                Official = new BluelyticsDto.OficialDto
                {
                    Buy = bluelyticsResponse.Oficial.ValueBuy,
                    Sell = bluelyticsResponse.Oficial.ValueSell
                },
                Blue = new BluelyticsDto.BlueDto
                {
                    Buy = bluelyticsResponse.Blue.ValueBuy,
                    Sell = bluelyticsResponse.Blue.ValueSell
                }
            };

            return bluelyticsDto;
        }

        private static string GetCacheKey()
        {
            return "bluelytics:v1";
        }
    }
}