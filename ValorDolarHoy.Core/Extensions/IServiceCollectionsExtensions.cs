using System.Net.Http;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using ServiceStack.Redis;
using ValorDolarHoy.Core.Clients.Currency;
using ValorDolarHoy.Core.Common.Storage;
using ValorDolarHoy.Core.Services.Currency;
using ValorDolarHoy.Mappings;

namespace ValorDolarHoy.Core.Extensions;

public static class IServiceCollectionsExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICurrencyService, CurrencyService>();
        services.AddSingleton<IKeyValueStore, RedisStore>();
        services.AddSingleton<IRedisClientsManagerAsync, PooledRedisClientManager>(_ =>
            new PooledRedisClientManager(configuration["Storage:Redis"]));
    }

    public static IServiceCollection AddClients(this IServiceCollection services)
    {
        services.AddHttpClient<ICurrencyClient, CurrencyClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                MaxConnectionsPerServer = 20
            })
            .AddPolicyHandler(Policy.BulkheadAsync<HttpResponseMessage>(20, int.MaxValue));

        return services;
    }

    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        MapperConfiguration mapperConfiguration = new(configure => { configure.AddProfile(new MappingProfile()); });
        IMapper mapper = mapperConfiguration.CreateMapper();
        services.AddSingleton(mapper);

        return services;
    }
}