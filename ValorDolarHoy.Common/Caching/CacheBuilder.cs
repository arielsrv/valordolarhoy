using System;
using Microsoft.Extensions.Caching.Memory;

namespace ValorDolarHoy.Common.Caching
{
    public class CacheBuilder<TKey, TValue>
    {
        private IMemoryCache memoryCache;
        private TimeSpan timeSpan;
        private int size;

        public static CacheBuilder<TKey, TValue> NewBuilder()
        {
            return new CacheBuilder<TKey, TValue>();
        }

        public CacheBuilder<TKey, TValue> ExpireAfterWrite(TimeSpan timeSpan)
        {
            this.timeSpan = timeSpan;
            return this;
        }

        public CacheBuilder<TKey, TValue> Size(int size)
        {
            this.size = size;
            return this;
        }

        public Cache<TKey, TValue> Build()
        {
            this.memoryCache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = this.size
            });

            return new Cache<TKey, TValue>(this.memoryCache, this.timeSpan, this.size);
        }
    }

    public class Cache<TKey, TValue>
    {
        private readonly IMemoryCache memoryCache;
        private readonly TimeSpan timeSpan;
        private readonly int size;

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
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(this.size)
                .SetPriority(CacheItemPriority.High)
                .SetAbsoluteExpiration(this.timeSpan);

            this.memoryCache.Set(key, value, cacheEntryOptions);
        }
    }
}