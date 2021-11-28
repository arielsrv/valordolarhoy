using System;
using System.Reactive.Linq;
using Moq;
using NUnit.Framework;
using ValorDolarHoy.Services;
using ValorDolarHoy.Services.Clients;

namespace ValorDolarHoy.Test.Services.Bluelytics
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
            this.bluelyticsClient.Setup(client => client.Get()).Returns(GetLatest());

            BluelyticsService bluelyticsService = new(this.bluelyticsClient.Object);

            BluelyticsDto bluelyticsDto = bluelyticsService.GetLatest().Wait();

            Assert.NotNull(bluelyticsDto);
            Assert.AreEqual(10.0M, bluelyticsDto.Official.Buy);
            Assert.AreEqual(11.0M, bluelyticsDto.Official.Sell);
            Assert.AreEqual(12.0M, bluelyticsDto.Blue.Buy);
            Assert.AreEqual(13.0M, bluelyticsDto.Blue.Sell);
        }

        private static IObservable<BluelyticsResponse> GetLatest()
        {
            return Observable.Return(new BluelyticsResponse
            {
                Oficial = new Oficial
                {
                    ValueBuy = 10.0M,
                    ValueSell = 11.0M
                },
                Blue = new Blue
                {
                    ValueBuy = 12.0M,
                    ValueSell = 13.0M
                }
            });
        }
    }
}