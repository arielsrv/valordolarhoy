using System;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using AutoMapper;
using ValorDolarHoy.Core.Clients.Currency;
using ValorDolarHoy.Core.Common.Caching;
using ValorDolarHoy.Core.Common.Storage;
using ValorDolarHoy.Core.Common.Threading;

#pragma warning disable CS1591

namespace ValorDolarHoy.Core.Services.Currency;

public interface ICurrencyService
{
    IObservable<CurrencyDto> GetLatest();
    IObservable<CurrencyDto> GetFallback();
    IObservable<string> GetAll();
}

public class CurrencyService(
    ICurrencyClient currencyClient,
    IKeyValueStore keyValueStore,
    IMapper mapper)
    : ICurrencyService
{
    private readonly ExecutorService _executorService = Executors.NewFixedThreadPool(10);

    public ICache<string, CurrencyDto> AppCache { get; init; } = CacheBuilder<string, CurrencyDto>
        .NewBuilder()
        .Size(2)
        .ExpireAfterWrite(TimeSpan.FromMinutes(1))
        .Build();

    public IObservable<CurrencyDto> GetLatest()
    {
        var cacheKey = GetCacheKey();

        CurrencyDto? currencyDto = this.AppCache.GetIfPresent(cacheKey);

        return currencyDto != null
            ? Observable.Return(currencyDto)
            : this.GetFromApi().Map(response =>
            {
                this._executorService.Run(() => this.AppCache.Put(cacheKey, response));
                return response;
            });
    }

    public IObservable<CurrencyDto> GetFallback()
    {
        var cacheKey = GetCacheKey();

        return keyValueStore.Get<CurrencyDto>(cacheKey).FlatMap(currencyDto =>
        {
            return currencyDto != null
                ? Observable.Return(currencyDto)
                : this.GetFromApi().Map(response =>
                {
                    this._executorService.Run(() =>
                        keyValueStore.Put(cacheKey, response, 60 * 10).ToBlocking()); // mm * ss
                    return response;
                });
        });
    }

    public IObservable<string> GetAll()
    {
        IObservable<CurrencyResponse> client1Observable = currencyClient.Get();
        IObservable<CurrencyResponse> client2Observable = currencyClient.Get();

        return client1Observable.Zip(client2Observable, (currencyResponse1, currencyResponse2) =>
        {
            var message =
                $"Oficial: {currencyResponse1.Oficial!.ValueSell}, Blue: {currencyResponse2.Blue!.ValueSell}";

            return message;
        });
    }

    private IObservable<CurrencyDto> GetFromApi()
    {
        return currencyClient.Get()
            .Map(mapper.Map<CurrencyDto>);
    }

    private static string GetCacheKey()
    {
        return "bluelytics:v1";
    }
}