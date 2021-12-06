using System;
using System.Reactive.Linq;
using System.Threading;
using Moq;
using ServiceStack.Caching;
using ServiceStack.Redis;
using ValorDolarHoy.Common.Storage;
using Xunit;

namespace ValorDolarHoy.Test.Storage
{
    public class KvsStoreTest
    {
        private readonly Mock<ICacheClientAsync> redisClient;
        private readonly Mock<IRedisClientsManagerAsync> redisClientManagerAsync;

        public KvsStoreTest()
        {
            this.redisClientManagerAsync = new Mock<IRedisClientsManagerAsync>();
            this.redisClient = new Mock<ICacheClientAsync>();
        }

        [Fact]
        public void Get()
        {
            this.redisClientManagerAsync
                .Setup(redisClientsManagerAsync => redisClientsManagerAsync.GetCacheClientAsync(CancellationToken.None))
                .ReturnsAsync(redisClient.Object);

            this.redisClient.Setup(client => client.GetAsync<string>("key", CancellationToken.None))
                .ReturnsAsync("value");

            IKvsStore kvsStore = new KvsStore(this.redisClientManagerAsync.Object);
            string actual = kvsStore.Get<string>("key").Wait();

            this.redisClient.Verify(mock => mock.GetAsync<string>("key", CancellationToken.None), Times.Once);
            Assert.Equal("value", actual);
        }

        [Fact]
        public void Add()
        {
            this.redisClientManagerAsync
                .Setup(redisClientsManagerAsync => redisClientsManagerAsync.GetCacheClientAsync(CancellationToken.None))
                .ReturnsAsync(redisClient.Object);

            this.redisClient.Setup(client => client.SetAsync("key", "value", CancellationToken.None))
                .ReturnsAsync(true);

            IKvsStore kvsStore = new KvsStore(this.redisClientManagerAsync.Object);
            kvsStore.Put("key", "value").Wait();

            this.redisClient.Verify(mock => mock.SetAsync("key", "value", TimeSpan.Zero, CancellationToken.None),
                Times.Once);
        }
    }
}