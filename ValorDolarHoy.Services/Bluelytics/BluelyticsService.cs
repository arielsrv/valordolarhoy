using System;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using ValorDolarHoy.Common.Caching;
using ValorDolarHoy.Common.Storage;
using ValorDolarHoy.Common.Thread;
using ValorDolarHoy.Services.Clients;

namespace ValorDolarHoy.Services
{
    public class BluelyticsService
    {
        private readonly IBluelyticsClient bluelyticsClient;

        private readonly ExecutorService executorService = ExecutorService.NewFixedThreadPool(10);

        private readonly IKvsStore kvsStore;

        public ICache<string, CurrencyDto> appCache = CacheBuilder<string, CurrencyDto>
            .NewBuilder()
            .Size(2)
            .ExpireAfterWrite(TimeSpan.FromMinutes(1))
            .Build();

        public BluelyticsService(IBluelyticsClient bluelyticsClient, IKvsStore kvsStore)
        {
            this.bluelyticsClient = bluelyticsClient;
            this.kvsStore = kvsStore;
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
                        Buy = bluelyticsResponse.oficial.ValueBuy,
                        Sell = bluelyticsResponse.oficial.ValueSell
                    },
                    Blue = new CurrencyDto.BlueDto
                    {
                        Buy = bluelyticsResponse.blue.ValueBuy,
                        Sell = bluelyticsResponse.blue.ValueSell
                    }
                };

                return currencyDto;
            });
        }

        private static string GetCacheKey()
        {
            return "bluelytics:v1";
        }

        public IObservable<CurrencyDto> GetFallback()
        {
            string cacheKey = GetCacheKey();

            return this.kvsStore.Get<CurrencyDto>(cacheKey).FlatMap(currencyDto =>
            {
                return currencyDto != null
                    ? Observable.Return(currencyDto)
                    : GetFromApi().Map(response =>
                    {
                        this.executorService.Run(() => this.kvsStore.Put(cacheKey, response).Wait());
                        return response;
                    });
            });
        }
    }
}