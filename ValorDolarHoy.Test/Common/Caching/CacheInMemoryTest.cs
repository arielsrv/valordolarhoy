using System;
using System.Threading;
using ValorDolarHoy.Core.Common.Caching;
using Xunit;

namespace ValorDolarHoy.Test.Common.Caching;

public class CacheInMemoryTest
{
    [Fact]
    public void Hit()
    {
        ICache<string, string> appCache = CacheBuilder<string, string>
            .NewBuilder()
            .Size(2)
            .ExpireAfterWrite(TimeSpan.FromMinutes(int.MaxValue))
            .Build();

        appCache.Put("key", "value");

        string? actual = appCache.GetIfPresent("key");

        Assert.NotNull(actual);
        Assert.Equal("value", actual);
    }

    [Fact]
    public void Miss()
    {
        ICache<string, string> appCache = CacheBuilder<string, string>
            .NewBuilder()
            .Size(2)
            .ExpireAfterWrite(TimeSpan.FromMinutes(int.MaxValue))
            .Build();

        string? actual = appCache.GetIfPresent("key");

        Assert.Null(actual);
    }

    [Fact]
    public void Expire_After_Write()
    {
        ICache<string, string> appCache = CacheBuilder<string, string>
            .NewBuilder()
            .Size(2)
            .ExpireAfterWrite(TimeSpan.FromMilliseconds(100))
            .Build();

        appCache.Put("key", "value");

        Thread.Sleep(200);

        string? actual = appCache.GetIfPresent("key");

        Assert.Null(actual);
    }

    [Fact]
    public void Size()
    {
        ICache<string, string> appCache = CacheBuilder<string, string>
            .NewBuilder()
            .Size(1)
            .ExpireAfterWrite(TimeSpan.FromMinutes(int.MaxValue))
            .Build();

        appCache.Put("key1", "value1");
        appCache.Put("key2", "value2");

        Thread.Sleep(200);

        string? actual = appCache.GetIfPresent("key1");

        Assert.Null(actual);
    }
}