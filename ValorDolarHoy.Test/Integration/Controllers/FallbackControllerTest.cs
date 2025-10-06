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
using ValorDolarHoy.Core;
using ValorDolarHoy.Core.Services.Currency;
using Xunit;

namespace ValorDolarHoy.Test.Integration.Controllers;

public class FallbackControllerTest
{
    private readonly Mock<ICurrencyService> currencyService;
    private readonly HttpClient httpClient;

    public FallbackControllerTest()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environments.Development);

        this.currencyService = new Mock<ICurrencyService>();

        IHostBuilder hostBuilder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.UseStartup<Startup>();
                webHost.ConfigureTestServices(services => { services.AddSingleton(_ => this.currencyService.Object); });
            });

        IHost host = hostBuilder.Start();

        this.httpClient = host.GetTestClient();
    }

    [Fact]
    public async Task Basic_Integration_Test_OkAsync()
    {
        this.currencyService.Setup(service => service.GetFallback())
            .Returns(GetLatest());

        HttpResponseMessage httpResponseMessage = await this.httpClient.GetAsync("/Fallback");
        var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
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