using System;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using ValorDolarHoy.Clients.Currency;
using ValorDolarHoy.Common.Caching;
using ValorDolarHoy.Common.Storage;
using ValorDolarHoy.Common.Threading;

namespace ValorDolarHoy.Services.Currency
{
    public class CurrencyService
    {
        private readonly ICurrencyClient currencyClient;

        private readonly ExecutorService executorService = ExecutorService.NewFixedThreadPool(10);

        private readonly IKeyValueStore keyValueStore;

        public ICache<string, CurrencyDto> appCache = CacheBuilder<string, CurrencyDto>
            .NewBuilder()
            .Size(2)
            .ExpireAfterWrite(TimeSpan.FromMinutes(1))
            .Build();

        public CurrencyService(
            ICurrencyClient currencyClient,
            IKeyValueStore keyValueStore
        )
        {
            this.currencyClient = currencyClient;
            this.keyValueStore = keyValueStore;
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
            return this.currencyClient.Get().Map(currencyResponse =>
            {
                CurrencyDto currencyDto = new()
                {
                    Official = new CurrencyDto.OficialDto
                    {
                        Buy = currencyResponse.oficial.ValueBuy,
                        Sell = currencyResponse.oficial.ValueSell
                    },
                    Blue = new CurrencyDto.BlueDto
                    {
                        Buy = currencyResponse.blue.ValueBuy,
                        Sell = currencyResponse.blue.ValueSell
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

            return this.keyValueStore.Get<CurrencyDto>(cacheKey).FlatMap(currencyDto =>
            {
                return currencyDto != null
                    ? Observable.Return(currencyDto)
                    : GetFromApi().Map(response =>
                    {
                        this.executorService.Run(() => this.keyValueStore.Put(cacheKey, response, 60 * 1).Wait()); // mm * ss
                        return response;
                    });
            });
        }
    }
}