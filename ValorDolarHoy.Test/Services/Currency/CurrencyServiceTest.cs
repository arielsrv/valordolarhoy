using System;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Moq;
using ValorDolarHoy.Common.Caching;
using ValorDolarHoy.Common.Storage;
using ValorDolarHoy.Services.Clients.Currency;
using ValorDolarHoy.Services.Currency;
using Xunit;

namespace ValorDolarHoy.Test.Services.Currency
{
    public class CurrencyServiceTest
    {
        private readonly Mock<ICache<string, CurrencyDto>> appCache;
        private readonly Mock<ICurrencyClient> currencyClient;
        private readonly Mock<IKvsStore> kvsStore;

        public CurrencyServiceTest()
        {
            this.currencyClient = new Mock<ICurrencyClient>();
            this.appCache = new Mock<ICache<string, CurrencyDto>>();
            this.kvsStore = new Mock<IKvsStore>();
        }

        [Fact]
        public void Get_Latest_Ok()
        {
            this.currencyClient.Setup(client => client.Get()).Returns(GetLatest());

            CurrencyService currencyService = new(this.currencyClient.Object, this.kvsStore.Object);

            CurrencyDto currencyDto = currencyService.GetLatest().ToBlockingFirst();

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

            CurrencyService currencyService = new(this.currencyClient.Object, this.kvsStore.Object)
            {
                appCache = this.appCache.Object
            };
            
            CurrencyDto currencyDto = currencyService.GetLatest().ToBlockingFirst();

            Assert.NotNull(currencyDto);
            Assert.Equal(10.0M, currencyDto.Official.Buy);
            Assert.Equal(11.0M, currencyDto.Official.Sell);
            Assert.Equal(12.0M, currencyDto.Blue.Buy);
            Assert.Equal(13.0M, currencyDto.Blue.Sell);
        }

        [Fact]
        public void Get_Latest_Fallback()
        {
            this.kvsStore.Setup(store => store.Get<CurrencyDto>("bluelytics:v1"))
                .Returns(Observable.Return(GetFromCache()));

            CurrencyService currencyService = new(this.currencyClient.Object, this.kvsStore.Object);

            CurrencyDto currencyDto = currencyService.GetFallback().ToBlockingFirst();

            Assert.NotNull(currencyDto);
            Assert.Equal(10.0M, currencyDto.Official.Buy);
            Assert.Equal(11.0M, currencyDto.Official.Sell);
            Assert.Equal(12.0M, currencyDto.Blue.Buy);
            Assert.Equal(13.0M, currencyDto.Blue.Sell);
        }

        [Fact]
        public void Get_Latest_Ok_Fallback_FromApi()
        {
            this.kvsStore.Setup(store => store.Get<CurrencyDto>("bluelytics:v1"))
                .Returns(Observable.Return(default(CurrencyDto)));
            
            this.currencyClient.Setup(client => client.Get()).Returns(GetLatest());

            CurrencyService currencyService = new(this.currencyClient.Object, this.kvsStore.Object);

            CurrencyDto currencyDto = currencyService.GetFallback().ToBlockingFirst();

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

        private static IObservable<CurrencyResponse> GetLatest()
        {
            return Observable.Return(new CurrencyResponse
            {
                oficial = new CurrencyResponse.Oficial
                {
                    ValueBuy = 10.0M,
                    ValueSell = 11.0M
                },
                blue = new CurrencyResponse.Blue
                {
                    ValueBuy = 12.0M,
                    ValueSell = 13.0M
                }
            });
        }
    }
}