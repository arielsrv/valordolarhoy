using System;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using AutoMapper;
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
    private readonly IMapper mapper;

    public CurrencyService(
        ICurrencyClient currencyClient,
        IKeyValueStore keyValueStore,
        IMapper mapper
    )
    {
        this.currencyClient = currencyClient;
        this.keyValueStore = keyValueStore;
        this.mapper = mapper;
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
                        this.keyValueStore.Put(cacheKey, response, 60 * 10).ToBlocking()); // mm * ss
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

    private IObservable<CurrencyDto> GetFromApi()
    {
        return this.currencyClient.Get()
            .Map(currencyResponse => this.mapper.Map<CurrencyDto>(currencyResponse));
    }

    private static string GetCacheKey()
    {
        return "bluelytics:v1";
    }
}