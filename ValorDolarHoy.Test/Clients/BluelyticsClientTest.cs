using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using Moq;
using NUnit.Framework;
using ValorDolarHoy.Common.Exceptions;
using ValorDolarHoy.Services;
using ValorDolarHoy.Services.Clients;

namespace ValorDolarHoy.Test.Clients
{
    [TestFixture]
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
        public void Get_Latest_Ok()
        {
            this.httpClient
                .Setup(client => client.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetResponse());

            BluelyticsClient bluelyticsClient = new(this.httpClient.Object);
            BluelyticsResponse bluelyticsResponse = bluelyticsClient.Get().Wait();

            Assert.NotNull(bluelyticsResponse);
            Assert.NotNull(bluelyticsResponse.Oficial);
            Assert.NotNull(bluelyticsResponse.Oficial.ValueSell);
            Assert.AreEqual(bluelyticsResponse.Oficial.ValueSell, 105.96);
        }

        [Test]
        public void Get_Latest_Not_Found()
        {
            this.httpClient
                .Setup(client => client.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            BluelyticsClient bluelyticsClient = new(this.httpClient.Object);
            
            Assert.Throws<ApiNotFoundException>(() => bluelyticsClient.Get().Wait());
        }

        [Test]
        public void Get_Latest_Generic_Error()
        {
            this.httpClient
                .Setup(client => client.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            BluelyticsClient bluelyticsClient = new(this.httpClient.Object);

            Assert.Throws<ApiException>(() => bluelyticsClient.Get().Wait());
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