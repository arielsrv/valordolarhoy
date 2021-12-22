using System;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using ValorDolarHoy.Core.Clients.Currency;
using ValorDolarHoy.Core.Common.Caching;
using ValorDolarHoy.Core.Common.Storage;
using ValorDolarHoy.Core.Common.Threading;

namespace ValorDolarHoy.Core.Services.Currency;

public interface ICurrencyService
{
    IObservable<CurrencyDto> GetLatest();
    IObservable<CurrencyDto> GetFallback();
    IObservable<string> GetAll();
}

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyClient currencyClient;

    private readonly ExecutorService executorService = Executors.NewFixedThreadPool(10);

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

        CurrencyDto? currencyDto = this.AppCache.GetIfPresent(cacheKey);

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
                Official = new OficialDto
                {
                    Buy = currencyResponse.Oficial!.ValueBuy,
                    Sell = currencyResponse.Oficial.ValueSell
                },
                Blue = new BlueDto
                {
                    Buy = currencyResponse.Blue!.ValueBuy,
                    Sell = currencyResponse.Blue.ValueSell
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
                $"Oficial: {currencyResponse1.Oficial!.ValueSell}, Blue: {currencyResponse2.Blue!.ValueSell}";

            return message;
        });
    }
}