using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ValorDolarHoy.Core;
using ValorDolarHoy.Core.Controllers;
using Xunit;

namespace ValorDolarHoy.Test.Unit;

public class StartupTest
{
    private readonly Mock<IConfiguration> configuration = new();

    [Fact]
    public void Currency()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        Startup startup = new(this.configuration.Object);
        startup.ConfigureServices(serviceCollection);
        serviceCollection.AddTransient<CurrencyController>();

        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        CurrencyController? controller = serviceProvider.GetService<CurrencyController>();
        Assert.NotNull(controller);
    }
}