using System;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using ValorDolarHoy.Core.Clients.Currency;
using ValorDolarHoy.Core.Common.Caching;
using ValorDolarHoy.Core.Common.Storage;
using ValorDolarHoy.Core.Common.Threading;

namespace ValorDolarHoy.Core.Services.Currency;

public class CurrencyService
{
    private readonly ICurrencyClient currencyClient;

    private readonly ExecutorService executorService = ExecutorService.NewFixedThreadPool(10);

    private readonly IKeyValueStore keyValueStore;

    public CurrencyService(
        ICurrencyClient currencyClient,
        IKeyValueStore keyValueStore
    )
    {
        this.currencyClient = currencyClient;
        this.keyValueStore = keyValueStore;
    }

    public ICache<string, CurrencyDto> AppCache { get; init; } = CacheBuilder<string, CurrencyDto>
        .NewBuilder()
        .Size(2)
        .ExpireAfterWrite(TimeSpan.FromMinutes(1))
        .Build();

    public IObservable<CurrencyDto> GetLatest()
    {
        string cacheKey = GetCacheKey();

        CurrencyDto currencyDto = this.AppCache.GetIfPresent(cacheKey);

        return currencyDto != null
            ? Observable.Return(currencyDto)
            : this.GetFromApi().Map(response =>
            {
                this.executorService.Run(() => this.AppCache.Put(cacheKey, response));
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
                : this.GetFromApi().Map(response =>
                {
                    this.executorService.Run(() =>
                        this.keyValueStore.Put(cacheKey, response, 60 * 10).Wait()); // mm * ss
                    return response;
                });
        });
    }

    public IObservable<string> GetAll()
    {
        IObservable<CurrencyResponse> client1Observable = this.currencyClient.Get();
        IObservable<CurrencyResponse> client2Observable = this.currencyClient.Get();
        
        return client1Observable.Zip(client2Observable, (currencyResponse1, currencyResponse2) =>
        {
            string message =
                $"Oficial: {currencyResponse1.oficial.ValueSell}, Blue: {currencyResponse2.blue.ValueSell}";

            return message;
        });
    }
}