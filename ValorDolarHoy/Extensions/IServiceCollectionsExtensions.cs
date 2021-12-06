using System.Net.Http;
using Polly;
using ServiceStack.Redis;
using ValorDolarHoy.Common.Storage;
using ValorDolarHoy.Services.Clients.Currency;
using ValorDolarHoy.Services.Currency;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionsExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<CurrencyService>();
            services.AddSingleton<IKeyValueStore, RedisStore>();
            services.AddSingleton<IRedisClientsManagerAsync, PooledRedisClientManager>(_ =>
                new PooledRedisClientManager("402639d6804af2a7bce70236e2ec3240@pike.redistogo.com:10753"));
        }

        public static void AddClients(this IServiceCollection services)
        {
            services.AddHttpClient<ICurrencyClient, CurrencyClient>()
                .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
                {
                    MaxConnectionsPerServer = 20
                })
                .AddPolicyHandler(Policy.BulkheadAsync<HttpResponseMessage>(20, int.MaxValue));
        }
    }
}