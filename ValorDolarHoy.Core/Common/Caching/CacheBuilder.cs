using Microsoft.Extensions.Caching.Memory;
using System;

namespace ValorDolarHoy.Core.Common.Caching;

public class CacheBuilder<TKey, TValue>
{
    private int size;
    private TimeSpan timeSpan;

    public static CacheBuilder<TKey, TValue> NewBuilder()
    {
        return new CacheBuilder<TKey, TValue>();
    }

    public CacheBuilder<TKey, TValue> ExpireAfterWrite(TimeSpan expireAfterWrite)
    {
        this.timeSpan = expireAfterWrite;
        return this;
    }

    public CacheBuilder<TKey, TValue> Size(int length)
    {
        this.size = length;
        return this;
    }

    public ICache<TKey, TValue> Build()
    {
        MemoryCache memoryCache = new(new MemoryCacheOptions { SizeLimit = this.size });
        return new Cache<TKey, TValue>(memoryCache, this.timeSpan, this.size);
    }
}

public interface ICache<in TKey, TValue>
{
    TValue GetIfPresent(TKey key);
    void Put(TKey key, TValue value);
}

public class Cache<TKey, TValue> : ICache<TKey, TValue>
{
    private readonly IMemoryCache memoryCache;
    private readonly int size;
    private readonly TimeSpan timeSpan;

    public Cache(IMemoryCache memoryCache, TimeSpan timeSpan, int size)
    {
        this.memoryCache = memoryCache;
        this.timeSpan = timeSpan;
        this.size = size;
    }

    public TValue GetIfPresent(TKey key)
    {
        return this.memoryCache.Get<TValue>(key);
    }

    public void Put(TKey key, TValue value)
    {
        MemoryCacheEntryOptions memoryCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSize(this.size)
            .SetPriority(CacheItemPriority.Low)
            .SetAbsoluteExpiration(this.timeSpan);

        this.memoryCache.Set(key, value, memoryCacheEntryOptions);
    }
}