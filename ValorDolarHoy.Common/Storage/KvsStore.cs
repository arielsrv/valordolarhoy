using System;
using System.Reactive;
using System.Reactive.Linq;
using ServiceStack.Caching;
using ServiceStack.Redis;

namespace ValorDolarHoy.Common.Storage
{
    public interface IKvsStore
    {
        IObservable<T> Get<T>(string key);
        IObservable<Unit> Put<T>(string key, T value, int seconds);
    }

    public class KvsStore : IKvsStore
    {
        private readonly IRedisClientsManager redisClientsManager;

        public KvsStore(IRedisClientsManager redisClientsManager)
        {
            this.redisClientsManager = redisClientsManager;
        }

        public IObservable<T> Get<T>(string key)
        {
            return Observable.Create(async (IObserver<T> observer) =>
            {
                using IRedisClientsManager clientsManager = this.redisClientsManager;
                await using ICacheClientAsync cacheClientAsync = await clientsManager.GetCacheClientAsync();
                T result = await cacheClientAsync.GetAsync<T>(key);

                observer.OnNext(result);
                observer.OnCompleted();
            });
        }

        public IObservable<Unit> Put<T>(string key, T value, int seconds)
        {
            return Observable.Create(async (IObserver<Unit> observer) =>
            {
                using IRedisClientsManager clientsManager = this.redisClientsManager;
                await using ICacheClientAsync cacheClientAsync = await clientsManager.GetCacheClientAsync();
                await cacheClientAsync.SetAsync(key, value, TimeSpan.FromSeconds(seconds));

                observer.OnNext(new Unit());
                observer.OnCompleted();
            });
        }
    }
}