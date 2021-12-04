using System;
using System.Reactive.Linq;
using Moq;
using ValorDolarHoy.Services;
using ValorDolarHoy.Services.Clients;
using Xunit;

namespace ValorDolarHoy.Test.Services.Bluelytics
{
    public class BluelyticsServiceTest
    {
        private readonly Mock<IBluelyticsClient> bluelyticsClient;
        
        public BluelyticsServiceTest()
        {
            this.bluelyticsClient = new Mock<IBluelyticsClient>();
        }

        [Fact]
        public void Get_Latest_Ok()
        {
            this.bluelyticsClient.Setup(client => client.Get()).Returns(GetLatest());

            BluelyticsService bluelyticsService = new(this.bluelyticsClient.Object);

            CurrencyDto currencyDto = bluelyticsService.GetLatest().Wait();

            Assert.NotNull(currencyDto);
            Assert.Equal(10.0M, currencyDto.Official.Buy);
            Assert.Equal(11.0M, currencyDto.Official.Sell);
            Assert.Equal(12.0M, currencyDto.Blue.Buy);
            Assert.Equal(13.0M, currencyDto.Blue.Sell);
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