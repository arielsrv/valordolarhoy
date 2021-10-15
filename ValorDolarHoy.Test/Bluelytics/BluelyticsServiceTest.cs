using Moq;
using NUnit.Framework;
using ValorDolarHoy.Services;
using ValorDolarHoy.Services.Clients;

namespace ValorDolarHoy.Test
{
    public class BluelyticsServiceTest
    {
        private Mock<IBluelyticsClient> bluelyticsClient;

        [SetUp]
        public void Setup()
        {
            this.bluelyticsClient = new Mock<IBluelyticsClient>();
        }

        [Test]
        public void Get_Latest_Ok()
        {
            this.bluelyticsClient.Setup(client => client.GetLatest()).ReturnsAsync(GetLatest());

            IBluelyticsService bluelyticsService = new BluelyticsService(this.bluelyticsClient.Object);

            BluelyticsDto bluelyticsDto = bluelyticsService.GetLatest().Result;

            Assert.NotNull(bluelyticsDto);
            Assert.AreEqual(10.0M, bluelyticsDto.official.buy);
            Assert.AreEqual(11.0M, bluelyticsDto.official.sell);
            Assert.AreEqual(12.0M, bluelyticsDto.blue.buy);
            Assert.AreEqual(13.0M, bluelyticsDto.blue.sell);
        }

        private static BluelyticsResponse GetLatest()
        {
            BluelyticsResponse bluelyticsResponse = new()
            {
                oficial = new BluelyticsResponse.Oficial
                {
                    valueBuy = 10.0M,
                    valueSell = 11.0M
                },
                blue = new BluelyticsResponse.Blue
                {
                    valueBuy = 12.0M,
                    valueSell = 13.0M
                }
            };

            return bluelyticsResponse;
        }
    }
}