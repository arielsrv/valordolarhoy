using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ValorDolarHoy.Core.Controllers;
using Xunit;

namespace ValorDolarHoy.Test.Unit;

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
        Startup startup = new(this.configuration.Object);
        startup.ConfigureServices(serviceCollection);
        serviceCollection.AddTransient<CurrencyController>();

        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        CurrencyController? controller = serviceProvider.GetService<CurrencyController>();
        Assert.NotNull(controller);
    }

    [Fact]
    public void Fallback()
    {
        this.configuration.SetupGet(config => config["Storage:Redis"]).Returns("redis.io");

        IServiceCollection serviceCollection = new ServiceCollection();
        Startup startup = new(this.configuration.Object);
        startup.ConfigureServices(serviceCollection);
        serviceCollection.AddTransient<FallbackController>();

        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        FallbackController? controller = serviceProvider.GetService<FallbackController>();
        Assert.NotNull(controller);
    }
}