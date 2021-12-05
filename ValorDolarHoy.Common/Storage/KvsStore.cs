using System;
using System.Reactive;
using System.Reactive.Linq;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace ValorDolarHoy.Common.Storage
{
    public interface IKvsStore
    {
        IObservable<T> Get<T>(string key);
        IObservable<Unit> Put<T>(string key, T value);
    }

    public class KvsStore : IKvsStore
    {
        private readonly IRedisCacheClient redisCacheClient;

        public KvsStore(IRedisCacheClient redisCacheClient)
        {
            this.redisCacheClient = redisCacheClient;
        }

        public IObservable<T> Get<T>(string key)
        {
            return Observable.Create(async (IObserver<T> observer) =>
            {
                T result = await this.redisCacheClient
                    .GetDbFromConfiguration()
                    .GetAsync<T>(key);

                observer.OnNext(result);
                observer.OnCompleted();
            });
        }

        public IObservable<Unit> Put<T>(string key, T value)
        {
            return Observable.Create(async (IObserver<Unit> observer) =>
            {
                await this.redisCacheClient
                    .GetDbFromConfiguration()
                    .AddAsync<T>(key, value);

                observer.OnNext(new Unit());
                observer.OnCompleted();
            });
        }
    }
}