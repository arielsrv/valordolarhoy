using System;
using System.Reactive.Linq;
using Moq;
using ValorDolarHoy.Common.Caching;
using ValorDolarHoy.Services;
using ValorDolarHoy.Services.Clients;
using Xunit;

namespace ValorDolarHoy.Test.Services.Bluelytics
{
    public class BluelyticsServiceTest
    {
        private readonly Mock<IBluelyticsClient> bluelyticsClient;
        private readonly Mock<ICache<string, CurrencyDto>> appCache;

        public BluelyticsServiceTest()
        {
            this.bluelyticsClient = new Mock<IBluelyticsClient>();
            this.appCache = new Mock<ICache<string, CurrencyDto>>();
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

        [Fact]
        public void Get_Latest_Ok_From_Cache()
        {
            this.appCache.Setup(client => client.GetIfPresent("bluelytics:v1")).Returns(GetFromCache());

            BluelyticsService bluelyticsService = new(this.bluelyticsClient.Object)
            {
                appCache = this.appCache.Object
            };

            CurrencyDto currencyDto = bluelyticsService.GetLatest().Wait();

            Assert.NotNull(currencyDto);
            Assert.Equal(10.0M, currencyDto.Official.Buy);
            Assert.Equal(11.0M, currencyDto.Official.Sell);
            Assert.Equal(12.0M, currencyDto.Blue.Buy);
            Assert.Equal(13.0M, currencyDto.Blue.Sell);
        }

        private static CurrencyDto GetFromCache()
        {
            return new CurrencyDto
            {
                Official = new CurrencyDto.OficialDto
                {
                    Buy = 10.0M,
                    Sell = 11.0M
                },
                Blue = new CurrencyDto.BlueDto
                {
                    Buy = 12.0M,
                    Sell = 13.0M
                }
            };
        }

        private static IObservable<BluelyticsResponse> GetLatest()
        {
            return Observable.Return(new BluelyticsResponse
            {
                oficial = new BluelyticsResponse.Oficial
                {
                    ValueBuy = 10.0M,
                    ValueSell = 11.0M
                },
                blue = new BluelyticsResponse.Blue
                {
                    ValueBuy = 12.0M,
                    ValueSell = 13.0M
                }
            });
        }
    }
}