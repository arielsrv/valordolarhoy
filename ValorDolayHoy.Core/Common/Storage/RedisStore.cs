using System;
using System.Reactive;
using System.Reactive.Linq;
using ServiceStack.Caching;
using ServiceStack.Redis;

namespace ValorDolarHoy.Common.Storage;

public class RedisStore : IKeyValueStore
{
    private readonly IRedisClientsManagerAsync redisClientsManagerAsync;

    public RedisStore(IRedisClientsManagerAsync redisClientsManagerAsync)
    {
        this.redisClientsManagerAsync = redisClientsManagerAsync;
    }

    public IObservable<T> Get<T>(string key)
    {
        return Observable.Create(async (IObserver<T> observer) =>
        {
            await using ICacheClientAsync cacheClientAsync = await this.redisClientsManagerAsync.GetCacheClientAsync();
            T result = await cacheClientAsync.GetAsync<T>(key);

            observer.OnNext(result);
            observer.OnCompleted();
        });
    }

    public IObservable<Unit> Put<T>(string key, T value)
    {
        return this.Put(key, value, TimeSpan.Zero.Seconds);
    }

    public IObservable<Unit> Put<T>(string key, T value, int seconds)
    {
        return Observable.Create(async (IObserver<Unit> observer) =>
        {
            await using ICacheClientAsync cacheClientAsync = await this.redisClientsManagerAsync.GetCacheClientAsync();
            await cacheClientAsync.SetAsync(key, value, TimeSpan.FromSeconds(seconds));

            observer.OnNext(new Unit());
            observer.OnCompleted();
        });
    }
}