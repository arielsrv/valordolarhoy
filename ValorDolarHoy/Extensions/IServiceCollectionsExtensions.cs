using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Polly;
using ServiceStack.Redis;
using ValorDolarHoy.Clients.Currency;
using ValorDolarHoy.Common.Storage;
using ValorDolarHoy.Services.Currency;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionsExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<CurrencyService>();
            services.AddSingleton<IKeyValueStore, RedisStore>();
            services.AddSingleton<IRedisClientsManagerAsync, PooledRedisClientManager>(_ =>
                new PooledRedisClientManager(configuration["Storage:Redis"]));

            return services;
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
    }
}