using System;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Redis;
using ValorDolarHoy.Core.Clients.Currency;
using ValorDolarHoy.Core.Common.Serialization;
using ValorDolarHoy.Core.Common.Storage;
using ValorDolarHoy.Core.Extensions;
using ValorDolarHoy.Core.Services.Currency;
using ValorDolarHoy.Mappings;

namespace ValorDolarHoy;

public class Startup : Application
{
    public Startup(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void Init(IServiceCollection services)
    {
        services.AddSingleton<ICurrencyService, CurrencyService>();
        services.AddSingleton<IKeyValueStore, RedisStore>();
        services.AddSingleton<IRedisClientsManagerAsync, PooledRedisClientManager>(_ =>
            new PooledRedisClientManager(configuration["Storage:Redis"]));

        services.AddHttpClient<ICurrencyClient, CurrencyClient>()
            .SetTimeout(TimeSpan.FromMilliseconds(1500))
            .SetMaxConnectionsPerServer(20)
            .SetMaxParallelization(20);
    }
}