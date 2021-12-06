using System;
using System.Reactive;

namespace ValorDolarHoy.Common.Storage
{
    public interface IKeyValueStore
    {
        IObservable<T> Get<T>(string key);
        IObservable<Unit> Put<T>(string key, T value);
        IObservable<Unit> Put<T>(string key, T value, int seconds);
    }
}