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

        private readonly Cache<string, CurrencyDto> appCache = CacheBuilder<string, CurrencyDto>
            .NewBuilder()
            .Size(2)
            .ExpireAfterWrite(TimeSpan.FromMinutes(1))
            .Build();

        private readonly ExecutorService executorService = ExecutorService.NewFixedThreadPool(10);

        public BluelyticsService(IBluelyticsClient bluelyticsClient)
        {
            this.bluelyticsClient = bluelyticsClient;
        }

        public IObservable<CurrencyDto> GetLatest()
        {
            string cacheKey = GetCacheKey();

            CurrencyDto currencyDto = this.appCache.GetIfPresent(cacheKey);

            return currencyDto != null
                ? Observable.Return(currencyDto)
                : GetFromApi().Map(response =>
                {
                    this.executorService.Run(() => this.appCache.Put(cacheKey, response));
                    return response;
                });
        }

        private IObservable<CurrencyDto> GetFromApi()
        {
            return this.bluelyticsClient.Get().Map(bluelyticsResponse =>
            {
                CurrencyDto currencyDto = new()
                {
                    Official = new CurrencyDto.OficialDto
                    {
                        Buy = bluelyticsResponse.Oficial.ValueBuy,
                        Sell = bluelyticsResponse.Oficial.ValueSell
                    },
                    Blue = new CurrencyDto.BlueDto
                    {
                        Buy = bluelyticsResponse.Blue.ValueBuy,
                        Sell = bluelyticsResponse.Blue.ValueSell
                    }
                };

                return currencyDto;
            });
        }

        private static string GetCacheKey()
        {
            return "bluelytics:v1";
        }
    }
}