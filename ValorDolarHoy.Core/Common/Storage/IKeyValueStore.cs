using System;
using System.Reactive;

#pragma warning disable CS1591

namespace ValorDolarHoy.Core.Common.Storage;

public interface IKeyValueStore
{
    IObservable<T?> Get<T>(string key);
    IObservable<Unit> Put<T>(string key, T value);
    IObservable<Unit> Put<T>(string key, T value, int seconds);
}