using System;
using System.Reactive.Linq;
using System.Threading;
using Moq;
using ServiceStack.Caching;
using ServiceStack.Redis;
using ValorDolarHoy.Core.Common.Storage;
using Xunit;

namespace ValorDolarHoy.Test.Unit.Common.Storage;

public class RedisStoreTest
{
    private readonly Mock<ICacheClientAsync> redisClient;
    private readonly Mock<IRedisClientsManagerAsync> redisClientManagerAsync;

    public RedisStoreTest()
    {
        this.redisClientManagerAsync = new Mock<IRedisClientsManagerAsync>();
        this.redisClient = new Mock<ICacheClientAsync>();
    }

    [Fact]
    public void Get()
    {
        this.redisClientManagerAsync
            .Setup(redisClientsManagerAsync => redisClientsManagerAsync.GetCacheClientAsync(CancellationToken.None))
            .ReturnsAsync(this.redisClient.Object);

        this.redisClient.Setup(client => client.GetAsync<string>("key", CancellationToken.None))
            .ReturnsAsync("value");

        RedisStore keyValueStore = new(this.redisClientManagerAsync.Object);
        string actual = keyValueStore.Get<string>("key").ToBlocking();

        this.redisClient.Verify(mock => mock.GetAsync<string>("key", CancellationToken.None), Times.Once);
        Assert.Equal("value", actual);
    }

    [Fact]
    public void Add()
    {
        this.redisClientManagerAsync
            .Setup(redisClientsManagerAsync => redisClientsManagerAsync.GetCacheClientAsync(CancellationToken.None))
            .ReturnsAsync(this.redisClient.Object);

        this.redisClient.Setup(client => client.SetAsync("key", "value", CancellationToken.None))
            .ReturnsAsync(true);

        RedisStore keyValueStore = new(this.redisClientManagerAsync.Object);
        keyValueStore.Put("key", "value").ToBlocking();

        this.redisClient.Verify(mock => mock.SetAsync("key", "value", TimeSpan.Zero, CancellationToken.None),
            Times.Once);
    }
}