using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Redis;
using ValorDolarHoy.Core.Clients.Currency;
using ValorDolarHoy.Core.Common.Extensions;
using ValorDolarHoy.Core.Common.Storage;
using ValorDolarHoy.Core.Services.Currency;

#pragma warning disable CS1591

namespace ValorDolarHoy;

public class Startup : Application
{
    /// <inheritdoc />
    public Startup(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void Init(IServiceCollection services)
    {
        services.AddSingleton<ICurrencyService, CurrencyService>();
        services.AddSingleton<IKeyValueStore, RedisStore>();
        services.AddSingleton<IRedisClientsManagerAsync, PooledRedisClientManager>(_ =>
            new PooledRedisClientManager(this.configuration["Storage:Redis"]));

        services.AddHttpClient<ICurrencyClient, CurrencyClient>()
            .WithAppSettings<CurrencyClient>(this.configuration);

        services.AddSingleton<IApplicationWarmUpper, ApplicationWarmupper>();
    }
}