using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Newtonsoft.Json;
using ValorDolarHoy.Core.Common.Exceptions;
using ValorDolarHoy.Core.Services.Currency;
using Xunit;

namespace ValorDolarHoy.Test.Integration.Controllers;

public class CurrencyControllerTest
{
    private readonly Mock<ICurrencyService> currencyService;
    private readonly HttpClient httpClient;

    public CurrencyControllerTest()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environments.Production);

        this.currencyService = new Mock<ICurrencyService>();

        IHostBuilder hostBuilder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.UseStartup<Startup>();
                webHost.ConfigureTestServices(services => { services.AddSingleton(_ => this.currencyService.Object); });
            });

        IHost? host = hostBuilder.Start();

        this.httpClient = host.GetTestClient();
    }

    [Fact]
    public async Task Basic_Integration_Test_OkAsync()
    {
        this.currencyService.Setup(service => service.GetLatest())
            .Returns(GetLatest());

        HttpResponseMessage httpResponseMessage = await this.httpClient.GetAsync("/Currency");
        string responseString = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);

        CurrencyDto? currencyDto = JsonConvert.DeserializeObject<CurrencyDto>(responseString);
        Assert.NotNull(currencyDto);
        Assert.NotNull(currencyDto.Official);
        Assert.Equal(10.0M, currencyDto.Official!.Buy);
        Assert.Equal(11.0M, currencyDto.Official.Sell);
        Assert.NotNull(currencyDto.Blue);
        Assert.Equal(12.0M, currencyDto.Blue!.Buy);
        Assert.Equal(13.0M, currencyDto.Blue.Sell);
    }

    [Fact]
    public async Task Basic_Integration_Test_InternalServerErrorAsync()
    {
        this.currencyService.Setup(service => service.GetLatest())
            .Returns(Observable.Throw<CurrencyDto>(new ApiException()));

        HttpResponseMessage httpResponseMessage = await this.httpClient.GetAsync("/Currency");
        string responseString = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);

        ErrorModel? errorModel = JsonConvert.DeserializeObject<ErrorModel>(responseString);

        Assert.NotNull(errorModel);
        Assert.Equal(500, errorModel.Code);
        Assert.Equal(nameof(ApiException), errorModel.Type);
    }

    [Fact]
    public async Task Basic_Integration_Test_ApiNotFoundAsync()
    {
        this.currencyService.Setup(service => service.GetLatest())
            .Returns(Observable.Throw<CurrencyDto>(new ApiNotFoundException("not found")));

        HttpResponseMessage httpResponseMessage = await this.httpClient.GetAsync("/Currency");
        string responseString = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);

        ErrorModel? errorModel = JsonConvert.DeserializeObject<ErrorModel>(responseString);

        Assert.NotNull(errorModel);
        Assert.Equal(404, errorModel.Code);
        Assert.Equal(nameof(ApiNotFoundException), errorModel.Type);
        Assert.Equal("not found", errorModel.Message);
    }

    [Fact]
    public async Task Basic_Integration_Test_BadRequestAsync()
    {
        this.currencyService.Setup(service => service.GetLatest())
            .Returns(Observable.Throw<CurrencyDto>(new ApiBadRequestException("bad request")));

        HttpResponseMessage httpResponseMessage = await this.httpClient.GetAsync("/Currency");
        string responseString = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);

        ErrorModel? errorModel = JsonConvert.DeserializeObject<ErrorModel>(responseString);

        Assert.NotNull(errorModel);
        Assert.Equal(400, errorModel.Code);
        Assert.Equal(nameof(ApiBadRequestException), errorModel.Type);
        Assert.Equal("bad request", errorModel.Message);
    }

    private static IObservable<CurrencyDto> GetLatest()
    {
        CurrencyDto currencyDto = new()
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

        return Observable.Return(currencyDto);
    }

    public class ErrorModel
    {
        public ErrorModel(int code, string? type, string? message)
        {
            this.Code = code;
            this.Type = type;
            this.Message = message;
        }

        public int Code { get; }
        public string? Type { get; }
        public string? Message { get; }
    }
}