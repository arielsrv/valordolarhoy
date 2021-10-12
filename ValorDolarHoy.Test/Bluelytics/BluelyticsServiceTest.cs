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
            Assert.AreEqual(12.0, bluelyticsDto.BlueDto.Buy);
            Assert.AreEqual(13.0, bluelyticsDto.BlueDto.Sell);
        }

        private static BluelyticsResponse GetLatest()
        {
            BluelyticsResponse bluelyticsResponse = new()
            {
                Blue = new Blue
                {
                    ValueAvg = 11.0,
                    ValueBuy = 12.0,
                    ValueSell = 13.0
                },
                Oficial = new Oficial
                {
                    ValueAvg = 11.0,
                    ValueBuy = 12.0,
                    ValueSell = 13.0
                }
            };

            return bluelyticsResponse;
        }
    }
}