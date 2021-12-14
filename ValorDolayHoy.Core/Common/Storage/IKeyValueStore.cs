using System;
using System.Reactive;

namespace ValorDolayHoy.Core.Common.Storage;

public interface IKeyValueStore
{
    IObservable<T> Get<T>(string key);
    IObservable<Unit> Put<T>(string key, T value);
    IObservable<Unit> Put<T>(string key, T value, int seconds);
}