using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ValorDolarHoy.Services;
using ValorDolarHoy.Services.Clients;

namespace ValorDolarHoy.Test.Clients
{
    public class BluelyticsClientTest
    {
        private Mock<HttpClient> httpClient;

        [SetUp]
        public void Setup()
        {
            Startup.JsonSerializerSettings();
            this.httpClient = new Mock<HttpClient>();
        }

        [Test]
        public async Task Get_Latest_OkAsync()
        {
            this.httpClient
                .Setup(client => client.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetResponse());

            BluelyticsClient bluelyticsClient = new(this.httpClient.Object);
            BluelyticsResponse bluelyticsResponse = await bluelyticsClient.GetLatestAsync();

            Assert.NotNull(bluelyticsResponse);
            Assert.NotNull(bluelyticsResponse.Oficial);
            Assert.NotNull(bluelyticsResponse.Oficial.ValueSell);
            Assert.AreEqual(bluelyticsResponse.Oficial.ValueSell, 105.96);
        }

        private static HttpResponseMessage GetResponse()
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(
                    "{\"oficial\":{\"value_avg\":102.96,\"value_sell\":105.96,\"value_buy\":99.96},\"blue\":{\"value_avg\":199.50,\"value_sell\":201.50,\"value_buy\":197.50},\"oficial_euro\":{\"value_avg\":110.71,\"value_sell\":113.94,\"value_buy\":107.48},\"blue_euro\":{\"value_avg\":214.52,\"value_sell\":216.67,\"value_buy\":212.37},\"last_update\":\"2021-11-19T19:55:35.460166-03:00\"}")
            };
        }
    }
}