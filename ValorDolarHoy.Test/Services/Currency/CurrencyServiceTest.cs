using System;
using System.Reactive.Linq;
using Moq;
using ValorDolarHoy.Core.Clients.Currency;
using ValorDolarHoy.Core.Common.Caching;
using ValorDolarHoy.Core.Common.Storage;
using ValorDolarHoy.Core.Services.Currency;
using Xunit;

namespace ValorDolarHoy.Test.Services.Currency;

public class CurrencyServiceTest
{
    private readonly Mock<ICache<string, CurrencyDto>> appCache;
    private readonly Mock<ICurrencyClient> currencyClient;
    private readonly Mock<IKeyValueStore> keyValueStore;

    public CurrencyServiceTest()
    {
        this.currencyClient = new Mock<ICurrencyClient>();
        this.appCache = new Mock<ICache<string, CurrencyDto>>();
        this.keyValueStore = new Mock<IKeyValueStore>();
    }

    [Fact]
    public void Get_Latest_Ok()
    {
        this.currencyClient.Setup(client => client.Get()).Returns(GetLatest());

        CurrencyService currencyService = new(this.currencyClient.Object, this.keyValueStore.Object);

        CurrencyDto currencyDto = currencyService.GetLatest().Wait();

        Assert.NotNull(currencyDto);
        Assert.NotNull(currencyDto.Official);
        Assert.Equal(10.0M, currencyDto.Official!.Buy);
        Assert.Equal(11.0M, currencyDto.Official.Sell);
        Assert.NotNull(currencyDto.Blue);
        Assert.Equal(12.0M, currencyDto.Blue!.Buy);
        Assert.Equal(13.0M, currencyDto.Blue.Sell);
    }

    [Fact]
    public void Get_All()
    {
        this.currencyClient.Setup(client => client.Get()).Returns(GetLatest());

        CurrencyService currencyService = new(this.currencyClient.Object, this.keyValueStore.Object);

        string actual = currencyService.GetAll().Wait();

        Assert.NotNull(actual);
        Assert.Equal("Oficial: 11.0, Blue: 13.0", actual);
    }

    [Fact]
    public void Get_Latest_Ok_From_Cache()
    {
        this.appCache.Setup(client => client.GetIfPresent("bluelytics:v1")).Returns(GetFromCache());

        CurrencyService currencyService = new(this.currencyClient.Object, this.keyValueStore.Object)
        {
            AppCache = this.appCache.Object
        };

        CurrencyDto currencyDto = currencyService.GetLatest().Wait();

        Assert.NotNull(currencyDto);
        Assert.Equal(10.0M, currencyDto.Official!.Buy);
        Assert.Equal(11.0M, currencyDto.Official.Sell);
        Assert.Equal(12.0M, currencyDto.Blue!.Buy);
        Assert.Equal(13.0M, currencyDto.Blue.Sell);
    }

    [Fact]
    public void Get_Latest_Fallback()
    {
        this.keyValueStore.Setup(store => store.Get<CurrencyDto>("bluelytics:v1"))
            .Returns(Observable.Return(GetFromCache()));

        CurrencyService currencyService = new(this.currencyClient.Object, this.keyValueStore.Object);

        CurrencyDto currencyDto = currencyService.GetFallback().Wait();

        Assert.NotNull(currencyDto);
        Assert.Equal(10.0M, currencyDto.Official!.Buy);
        Assert.Equal(11.0M, currencyDto.Official.Sell);
        Assert.Equal(12.0M, currencyDto.Blue!.Buy);
        Assert.Equal(13.0M, currencyDto.Blue.Sell);
    }

    [Fact]
    public void Get_Latest_Ok_Fallback_FromApi()
    {
        this.keyValueStore.Setup(store => store.Get<CurrencyDto>("bluelytics:v1"))
            .Returns(Observable.Return(default(CurrencyDto)));

        this.currencyClient.Setup(client => client.Get()).Returns(GetLatest());

        CurrencyService currencyService = new(this.currencyClient.Object, this.keyValueStore.Object);

        CurrencyDto currencyDto = currencyService.GetFallback().Wait();

        Assert.NotNull(currencyDto);
        Assert.Equal(10.0M, currencyDto.Official!.Buy);
        Assert.Equal(11.0M, currencyDto.Official.Sell);
        Assert.Equal(12.0M, currencyDto.Blue!.Buy);
        Assert.Equal(13.0M, currencyDto.Blue.Sell);
    }

    private static CurrencyDto GetFromCache()
    {
        return new CurrencyDto
        {
            Official = new OficialDto
            {
                Buy = 10.0M,
                Sell = 11.0M
            },
            Blue = new BlueDto
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
            Oficial = new OficialResponse
            {
                ValueBuy = 10.0M,
                ValueSell = 11.0M
            },
            Blue = new BlueResponse
            {
                ValueBuy = 12.0M,
                ValueSell = 13.0M
            }
        });
    }
}