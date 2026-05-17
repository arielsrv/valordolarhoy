using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ValorDolarHoy.Core.Clients.Currency;
using ValorDolarHoy.Core.Common.Extensions;
using ValorDolarHoy.Core.Services.Currency;

#pragma warning disable CS1591

namespace ValorDolarHoy.Core;

public class Startup : Application
{
    /// <inheritdoc />
    public Startup(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void Init(IServiceCollection services)
    {
        services.AddSingleton<ICurrencyService, CurrencyService>();

        services.AddHttpClient<ICurrencyClient, CurrencyClient>()
            .WithAppSettings<CurrencyClient>(this.Configuration);

        services.AddSingleton<IApplicationWarmUpper, ApplicationWarmupper>();
    }
}