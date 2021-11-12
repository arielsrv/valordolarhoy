using System;
using System.Threading.Tasks;
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
            this.appCache.Put(cacheKey, bluelyticsDto);

            return bluelyticsDto;
        }

        private async Task<BluelyticsDto> GetFromApi()
        {
            BluelyticsResponse bluelyticsResponse = await this.bluelyticsClient.GetLatest();

            BluelyticsDto bluelyticsDto = new()
            {
                Official = new BluelyticsDto.OficialDto
                {
                    Buy = bluelyticsResponse.oficial.ValueBuy,
                    Sell = bluelyticsResponse.oficial.ValueSell
                },
                Blue = new BluelyticsDto.BlueDto
                {
                    Buy = bluelyticsResponse.blue.ValueBuy,
                    Sell = bluelyticsResponse.blue.ValueSell
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