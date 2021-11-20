using System;
using System.Threading.Tasks;
using ValorDolarHoy.Services.Clients;
using ValorDolarHoy.Common.Caching;
using ValorDolarHoy.Common.Thread;

namespace ValorDolarHoy.Services
{
    public class BluelyticsService
    {
        private readonly BluelyticsClient bluelyticsClient;

        private readonly Cache<string, BluelyticsDto> appCache = CacheBuilder<string, BluelyticsDto>
            .NewBuilder()
            .Size(4)
            .ExpireAfterWrite(TimeSpan.FromMinutes(5))
            .Build();

        private readonly ExecutorService executorService = ExecutorService.NewFixedThreadPool(10);

        public BluelyticsService(BluelyticsClient bluelyticsClient)
        {
            this.bluelyticsClient = bluelyticsClient;
        }

        public async Task<BluelyticsDto> GetLatestAsync()
        {
            string cacheKey = GetCacheKey();

            BluelyticsDto bluelyticsDto = this.appCache.GetIfPresent(cacheKey);

            if (bluelyticsDto != null)
            {
                return bluelyticsDto;
            }

            bluelyticsDto = await GetFromApiAsync();

            this.executorService.Run(() => this.appCache.Put(cacheKey, bluelyticsDto));

            return bluelyticsDto;
        }

        private async Task<BluelyticsDto> GetFromApiAsync()
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