using System;
using System.Reactive.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using ValorDolarHoy.Core.Clients.Currency;
using ValorDolarHoy.Core.Common.Caching;
using ValorDolarHoy.Core.Common.Storage;
using ValorDolarHoy.Core.Services.Currency;
using ValorDolarHoy.Mappings;
using Xunit;

namespace ValorDolarHoy.Test.Unit.Services.Currency;

public class CurrencyServiceTest
{
    private readonly Mock<ICache<string, CurrencyDto>> _appCache;
    private readonly Mock<ICurrencyClient> _currencyClient;
    private readonly Mock<IKeyValueStore> _keyValueStore;
    private readonly IMapper _mapper;

    public CurrencyServiceTest()
    {
        this._currencyClient = new Mock<ICurrencyClient>();
        this._appCache = new Mock<ICache<string, CurrencyDto>>();
        this._keyValueStore = new Mock<IKeyValueStore>();
        LoggerFactory loggerFactory = new();
        MapperConfigurationExpression config = new MapperConfigurationExpression
        {
            LicenseKey = "DEMO-LICENSE-KEY-FOR-TESTING"
        };
        config.AddProfile(new MappingProfile());
        MapperConfiguration mapperConfiguration = new MapperConfiguration(config, loggerFactory);
        this._mapper = mapperConfiguration.CreateMapper();
    }

    [Fact]
    public void Get_Latest_Ok()
    {
        this._currencyClient.Setup(client => client.Get()).Returns(GetLatest());

        CurrencyService currencyService = new(this._currencyClient.Object, this._keyValueStore.Object, this._mapper);

        CurrencyDto currencyDto = currencyService.GetLatest().ToBlocking();

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
        this._currencyClient.Setup(client => client.Get()).Returns(GetLatest());

        CurrencyService currencyService = new(this._currencyClient.Object, this._keyValueStore.Object, this._mapper);

        var actual = currencyService.GetAll().ToBlocking();

        Assert.NotNull(actual);
        Assert.Equal("Oficial: 11, Blue: 13", actual);
    }

    [Fact]
    public void Get_Latest_Ok_From_Cache()
    {
        this._appCache.Setup(client => client.GetIfPresent("bluelytics:v1")).Returns(GetFromCache());

        CurrencyService currencyService = new(this._currencyClient.Object, this._keyValueStore.Object, this._mapper)
        {
            AppCache = this._appCache.Object
        };

        CurrencyDto currencyDto = currencyService.GetLatest().ToBlocking();

        Assert.NotNull(currencyDto);
        Assert.Equal(10.0M, currencyDto.Official!.Buy);
        Assert.Equal(11.0M, currencyDto.Official.Sell);
        Assert.Equal(12.0M, currencyDto.Blue!.Buy);
        Assert.Equal(13.0M, currencyDto.Blue.Sell);
    }

    [Fact]
    public void Get_Latest_Fallback()
    {
        this._keyValueStore.Setup(store => store.Get<CurrencyDto>("bluelytics:v1"))
            .Returns(Observable.Return(GetFromCache()));

        CurrencyService currencyService = new(this._currencyClient.Object, this._keyValueStore.Object, this._mapper);

        CurrencyDto currencyDto = currencyService.GetFallback().ToBlocking();

        Assert.NotNull(currencyDto);
        Assert.Equal(10.0M, currencyDto.Official!.Buy);
        Assert.Equal(11.0M, currencyDto.Official.Sell);
        Assert.Equal(12.0M, currencyDto.Blue!.Buy);
        Assert.Equal(13.0M, currencyDto.Blue.Sell);
    }

    [Fact]
    public void Get_Latest_Ok_Fallback_FromApi()
    {
        this._keyValueStore.Setup(store => store.Get<CurrencyDto>("bluelytics:v1"))
            .Returns(Observable.Return(default(CurrencyDto)));

        this._currencyClient.Setup(client => client.Get()).Returns(GetLatest());

        CurrencyService currencyService = new(this._currencyClient.Object, this._keyValueStore.Object, this._mapper);

        CurrencyDto currencyDto = currencyService.GetFallback().ToBlocking();

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
                Buy = 10,
                Sell = 11
            },
            Blue = new BlueDto
            {
                Buy = 12,
                Sell = 13
            }
        };
    }

    private static IObservable<CurrencyResponse> GetLatest()
    {
        return Observable.Return(new CurrencyResponse
        {
            Oficial = new OficialResponse
            {
                ValueBuy = 10,
                ValueSell = 11
            },
            Blue = new BlueResponse
            {
                ValueBuy = 12,
                ValueSell = 13
            }
        });
    }
}
