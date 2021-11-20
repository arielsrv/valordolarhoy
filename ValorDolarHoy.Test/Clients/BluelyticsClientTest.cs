using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ValorDolarHoy.Common;
using ValorDolarHoy.Services;
using ValorDolarHoy.Services.Clients;

namespace ValorDolarHoy.Test.Clients
{
    public class BluelyticsClientTest
    {
        private Mock<Client> client;

        [SetUp]
        public void Setup()
        {
            this.client = new Mock<Client>(new Mock<HttpClient>().Object);
        }

        [Test]
        public void Get_Latest_Ok()
        {
            this.client.Setup(x => x.Get<BluelyticsResponse>("https://api.bluelytics.com.ar/v2/latest"))
                .Returns(GetResponse());

            BluelyticsClient bluelyticsClient = new(this.client.Object);
            BluelyticsResponse bluelyticsResponse = bluelyticsClient.GetLatest().Result;

            Assert.NotNull(bluelyticsResponse);
        }

        private Task<BluelyticsResponse> GetResponse()
        {
            return Task.FromResult(new BluelyticsResponse());
        }
    }
}