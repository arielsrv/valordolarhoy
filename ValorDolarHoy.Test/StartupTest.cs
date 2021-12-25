using System;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Newtonsoft.Json;
using ValorDolarHoy.Core.Common.Exceptions;
using ValorDolarHoy.Core.Controllers;
using ValorDolarHoy.Core.Extensions;
using ValorDolarHoy.Core.Services.Currency;
using Xunit;

namespace ValorDolarHoy.Test;

public class StartupTest
{
    private readonly Mock<IConfiguration> configuration;

    public StartupTest()
    {
        this.configuration = new Mock<IConfiguration>();
    }

    [Fact]
    public void Currency()
    {
        this.configuration.SetupGet(config => config["Storage:Redis"]).Returns("redis.io");

        IServiceCollection serviceCollection = new ServiceCollection();
        Startup startup = new(configuration.Object);
        startup.ConfigureServices(serviceCollection);
        serviceCollection.AddTransient<CurrencyController>();

        ServiceProvider? serviceProvider = serviceCollection.BuildServiceProvider();
        CurrencyController? controller = serviceProvider.GetService<CurrencyController>();
        Assert.NotNull(controller);
    }

    [Fact]
    public void Fallback()
    {
        this.configuration.SetupGet(config => config["Storage:Redis"]).Returns("redis.io");

        IServiceCollection serviceCollection = new ServiceCollection();
        Startup startup = new(configuration.Object);
        startup.ConfigureServices(serviceCollection);
        serviceCollection.AddTransient<FallbackController>();

        ServiceProvider? serviceProvider = serviceCollection.BuildServiceProvider();
        FallbackController? controller = serviceProvider.GetService<FallbackController>();
        Assert.NotNull(controller);
    }


    [Fact]
    public async Task Basic_Integration_Test_OkAsync()
    {
        Mock<ICurrencyService> currencyService = new();
        currencyService.Setup(service => service.GetLatest())
            .Returns(GetLatest());

        IHostBuilder hostBuilder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.UseStartup<Startup>();
                webHost.ConfigureTestServices(services => { services.SwapTransient(_ => currencyService.Object); });
            });

        IHost? host = await hostBuilder.StartAsync();

        HttpClient httpClient = host.GetTestClient();

        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("/Currency");
        string responseString = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);

        CurrencyDto? currencyDto = JsonConvert.DeserializeObject<CurrencyDto>(responseString);
        Assert.NotNull(currencyDto);
        Assert.Equivalent(GetLatest(), currencyDto);
    }

    [Fact]
    public async Task Basic_Integration_Test_FailAsync()
    {
        Mock<ICurrencyService> currencyService = new();
        currencyService.Setup(service => service.GetLatest())
            .Returns(Observable.Throw<CurrencyDto>(new ApiException()));

        IHostBuilder hostBuilder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.UseStartup<Startup>();
                webHost.ConfigureTestServices(services => { services.SwapTransient(_ => currencyService.Object); });
            });

        IHost? host = await hostBuilder.StartAsync();

        HttpClient httpClient = host.GetTestClient();

        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("/Currency");
        string responseString = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);

        ErrorModel? errorModel = JsonConvert.DeserializeObject<ErrorModel>(responseString);

        Assert.NotNull(errorModel);
        Assert.Equal(500, errorModel.Code);
        Assert.Equal(nameof(ApiException), errorModel.Type);
    }

    public class ErrorModel
    {
        public ErrorModel(int code, string? type)
        {
            this.Code = code;
            this.Type = type;
        }

        public int Code { get; }
        public string? Type { get; }
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
}