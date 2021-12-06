using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ServiceStack.Redis;

namespace ValorDolarHoy.Common.Storage
{
    public interface IKvsStore
    {
        IObservable<T> Get<T>(string key);
        IObservable<Unit> Put<T>(string key, T value);
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
            return Observable.Create((IObserver<T> observer) =>
            {
                using IRedisClientsManager clientsManager = this.redisClientsManager;
                using IRedisClient redisClient = clientsManager.GetClient();
                T result = redisClient.Get<T>(key);

                observer.OnNext(result);
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

        public IObservable<Unit> Put<T>(string key, T value)
        {
            return this.Put(key, value, 0);
        }

        public IObservable<Unit> Put<T>(string key, T value, int seconds)
        {
            return Observable.Create((IObserver<Unit> observer) =>
            {
                using IRedisClientsManager clientsManager = this.redisClientsManager;
                using IRedisClient redisClient = clientsManager.GetClient();
                redisClient.Set(key, value, TimeSpan.FromSeconds(seconds));

                observer.OnNext(new Unit());
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }
    }
}