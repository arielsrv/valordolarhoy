using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ValorDolarHoy.Core.Controllers;
using ValorDolarHoy.Core.Services.Currency;
using Xunit;

namespace ValorDolarHoy.Test.Controllers;

public class CurrencyControllerTest
{
    private readonly Mock<ICurrencyService> currencyService;

    public CurrencyControllerTest()
    {
        this.currencyService = new Mock<ICurrencyService>();
    }

    [Fact]
    public async Task Get_LatestAsync()
    {
        this.currencyService.Setup(service => service.GetLatest()).Returns(GetLatest());

        CurrencyController currencyController = new(this.currencyService.Object);

        IActionResult actionResult = await currencyController.GetLatestAsync();

        Assert.NotNull(actionResult);
        Assert.IsType<OkObjectResult>(actionResult);

        OkObjectResult okObjectResult = (OkObjectResult)actionResult;
        Assert.Equal(200, okObjectResult.StatusCode);
        Assert.Equivalent(GetLatest().Wait(), okObjectResult.Value);
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