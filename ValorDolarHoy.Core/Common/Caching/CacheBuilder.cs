using System;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Caching.Memory;

#pragma warning disable CS1591

namespace ValorDolarHoy.Core.Common.Caching;

public class CacheBuilder<TKey, TValue>
{
    private int _size;
    private TimeSpan _timeSpan;

    public static CacheBuilder<TKey, TValue> NewBuilder()
    {
        return new CacheBuilder<TKey, TValue>();
    }

    public CacheBuilder<TKey, TValue> ExpireAfterWrite(TimeSpan expireAfterWrite)
    {
        Guard.Against.NegativeOrZero(expireAfterWrite);
        this._timeSpan = expireAfterWrite;
        return this;
    }

    public CacheBuilder<TKey, TValue> Size(int length)
    {
        Guard.Against.NegativeOrZero(length);
        this._size = length;
        return this;
    }

    public ICache<TKey, TValue> Build()
    {
        MemoryCache memoryCache = new(new MemoryCacheOptions
        {
            SizeLimit = this._size
        });

        MemoryCacheEntryOptions memoryCacheEntryOptions = new();
        memoryCacheEntryOptions
            .SetSize(1)
            .SetAbsoluteExpiration(this._timeSpan);

        return new Cache<TKey, TValue>(memoryCache, memoryCacheEntryOptions);
    }
}

public interface ICache<in TKey, TValue>
{
    TValue? GetIfPresent(TKey key);
    void Put(TKey key, TValue value);
}

public class Cache<TKey, TValue> : ICache<TKey, TValue>
{
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _memoryCacheEntryOptions;

    public Cache(IMemoryCache memoryCache, MemoryCacheEntryOptions memoryCacheEntryOptions)
    {
        Guard.Against.Null(memoryCache);
        Guard.Against.Null(memoryCacheEntryOptions);

        this._memoryCache = memoryCache;
        this._memoryCacheEntryOptions = memoryCacheEntryOptions;
    }

    public TValue? GetIfPresent(TKey key)
    {
        return this._memoryCache.Get<TValue>(key ?? throw new ArgumentNullException(nameof(key)));
    }

    public void Put(TKey key, TValue value)
    {
        this._memoryCache.Set(key ?? throw new ArgumentNullException(nameof(key)), value, this._memoryCacheEntryOptions);
    }
}
