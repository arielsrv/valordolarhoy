using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ValorDolarHoy.Core.Controllers;
using ValorDolarHoy.Core.Services.Currency;
using Xunit;

namespace ValorDolarHoy.Test.Controllers;

public class FallbackControllerTest
{
    private readonly Mock<ICurrencyService> currencyService;

    public FallbackControllerTest()
    {
        this.currencyService = new Mock<ICurrencyService>();
    }

    [Fact]
    public async Task Get_LatestAsync()
    {
        IObservable<CurrencyDto> observableCurrencyDto = GetLatest();

        this.currencyService.Setup(service => service.GetFallback()).Returns(observableCurrencyDto);

        FallbackController fallbackController = new(this.currencyService.Object);

        IActionResult actionResult = await fallbackController.GetLatestAsync();

        Assert.NotNull(actionResult);
        Assert.IsType<OkObjectResult>(actionResult);

        OkObjectResult okObjectResult = (OkObjectResult)actionResult;
        Assert.Equal(200, okObjectResult.StatusCode);
        Assert.Equivalent(observableCurrencyDto.Wait(), okObjectResult.Value);
    }

    [Fact]
    public async Task Get_ZipAsync()
    {
        IObservable<string> observableMessage = GetMessage();

        this.currencyService.Setup(service => service.GetAll()).Returns(observableMessage);

        FallbackController fallbackController = new(this.currencyService.Object);

        IActionResult actionResult = await fallbackController.ZipAsync();

        Assert.NotNull(actionResult);
        Assert.IsType<OkObjectResult>(actionResult);

        OkObjectResult okObjectResult = (OkObjectResult)actionResult;
        Assert.Equal(200, okObjectResult.StatusCode);
        Assert.Equivalent(observableMessage.Wait(), okObjectResult.Value);
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

    private static IObservable<string> GetMessage()
    {
        return Observable.Return("hello_world!");
    }
}