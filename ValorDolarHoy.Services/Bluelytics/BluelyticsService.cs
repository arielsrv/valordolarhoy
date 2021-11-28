using System;
using System.Reactive.Linq;
using ValorDolarHoy.Common.Caching;
using ValorDolarHoy.Common.Thread;
using ValorDolarHoy.Services.Clients;

namespace ValorDolarHoy.Services
{
    public class BluelyticsService
    {
        private readonly IBluelyticsClient bluelyticsClient;

        private readonly Cache<string, BluelyticsDto> appCache = CacheBuilder<string, BluelyticsDto>
            .NewBuilder()
            .Size(4)
            .ExpireAfterWrite(TimeSpan.FromSeconds(5))
            .Build();

        private readonly ExecutorService executorService = ExecutorService.NewFixedThreadPool(10);

        public BluelyticsService(IBluelyticsClient bluelyticsClient)
        {
            this.bluelyticsClient = bluelyticsClient;
        }

        public IObservable<BluelyticsDto> GetLatest()
        {
            string cacheKey = GetCacheKey();

            BluelyticsDto bluelyticsDto = this.appCache.GetIfPresent(cacheKey);

            return bluelyticsDto != null
                ? Observable.Return(bluelyticsDto)
                : GetFromApi().Map(response =>
                {
                    this.executorService.Run(() => this.appCache.Put(cacheKey, response));
                    return response;
                });
        }

        private IObservable<BluelyticsDto> GetFromApi()
        {
            return this.bluelyticsClient.Get().Map(bluelyticsResponse =>
            {
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
            });
        }

        private static string GetCacheKey()
        {
            return "bluelytics:v1";
        }
    }
}