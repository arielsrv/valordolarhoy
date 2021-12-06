using System.Reactive.Linq;
using Moq;
using ServiceStack.Redis;
using ValorDolarHoy.Common.Storage;
using Xunit;

namespace ValorDolarHoy.Test.Storage
{
    public class KvsStoreTest
    {
        private readonly Mock<IRedisClientsManager> redisClientManager;
        private readonly Mock<IRedisClient> redisClient;

        public KvsStoreTest()
        {
            this.redisClientManager = new Mock<IRedisClientsManager>();
            this.redisClient = new Mock<IRedisClient>();
        }

        [Fact]
        public void Get()
        {
            this.redisClientManager.Setup(x => x.GetClient()).Returns(redisClient.Object);
            this.redisClient.Setup(x => x.Get<string>("key")).Returns("value");

            IKvsStore kvsStore = new KvsStore(redisClientManager.Object);

            string actual = kvsStore.Get<string>("key").Wait();

            Assert.Equal("value", actual);
        }

        [Fact]
        public void Add()
        {
            this.redisClientManager.Setup(x => x.GetClient()).Returns(redisClient.Object);
            this.redisClient.Setup(x => x.Set("key", "value")).Returns(true);

            IKvsStore kvsStore = new KvsStore(redisClientManager.Object);

            kvsStore.Put("key", "value").Wait();
        }
    }
}