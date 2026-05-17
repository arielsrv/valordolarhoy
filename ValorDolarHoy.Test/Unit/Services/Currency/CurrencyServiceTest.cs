using System;
using System.Reactive.Linq;
using System.Threading;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using ValorDolarHoy.Core.Clients.Currency;
using ValorDolarHoy.Core.Common.Caching;
using ValorDolarHoy.Core.Services.Currency;
using ValorDolarHoy.Mappings;
using Xunit;

namespace ValorDolarHoy.Test.Unit.Services.Currency;

public class CurrencyServiceTest
{
    private readonly Mock<ICache<string, CurrencyDto>> _appCache;
    private readonly Mock<ICurrencyClient> _currencyClient;
    private readonly IMapper _mapper;

    public CurrencyServiceTest()
    {
        this._currencyClient = new Mock<ICurrencyClient>();
        this._appCache = new Mock<ICache<string, CurrencyDto>>();
        LoggerFactory loggerFactory = new();
        MapperConfigurationExpression config = new()
        {
            LicenseKey =
                "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODA2MzY0ODAwIiwiaWF0IjoiMTc3NDg2ODk0NiIsImFjY291bnRfaWQiOiIwMTk4MDJlYzI3Yzc3MTZlYjNmYWEwNDNlZWYyNmUzOSIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa216NnhxYXJ5ZWhnY3AwNXkybmpoZ3d2Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.2J42Nv91U_8RGu9ngau-hdEQhvfzNzuNugQrccDdrpbU-lzDjinvIzaw3lJZBm5ySK8g1u59s7dhcB8YWvbg-0f-rrv7HW7djcRBYxYkefEVy2wQV2xmADL_vhwQJPJniHnon_tk4twDfL97sB-BEZoG87NXukHTTll_JMR4lRXmay2olf7kJwYG6KmZzIT5x5vasZ9qEOu0EgNjQYponLyXZQ3Dox2fc-7OY8NqKvvbUFC-K8P5p0Weq_mqjxbu-GDL7IjMUDIEUDa13-riofJcGjgFFruLt5OIzsK9_LJWY-qB195WuzMX7fu2B1Y32eziqP0i4vLLC8jsjtr-SQ"
        };
        config.AddProfile(new MappingProfile());
        MapperConfiguration mapperConfiguration = new(config, loggerFactory);
        this._mapper = mapperConfiguration.CreateMapper();
    }

    [Fact]
    public void Get_Latest_Ok()
    {
        this._currencyClient.Setup(client => client.Get()).Returns(GetLatest());

        CurrencyService currencyService = new(this._currencyClient.Object, this._mapper);

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

        CurrencyService currencyService = new(this._currencyClient.Object, this._mapper);

        var actual = currencyService.GetAll().ToBlocking();

        Assert.NotNull(actual);
        Assert.Equal("Oficial: 11, Blue: 13", actual);
    }

    [Fact]
    public void Get_Latest_Ok_From_Cache()
    {
        this._appCache.Setup(client => client.GetIfPresent("bluelytics:v1")).Returns(GetFromCache());

        CurrencyService currencyService = new(this._currencyClient.Object, this._mapper)
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
    public void Get_Latest_Ok_Calls_Put_In_Cache()
    {
        this._currencyClient.Setup(client => client.Get()).Returns(GetLatest());

        CurrencyService currencyService = new(this._currencyClient.Object, this._mapper)
        {
            AppCache = this._appCache.Object
        };

        CurrencyDto currencyDto = currencyService.GetLatest().ToBlocking();

        Assert.NotNull(currencyDto);
        Thread.Sleep(200);
        this._appCache.Verify(cache => cache.Put("bluelytics:v1", It.IsAny<CurrencyDto>()), Times.Once);
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