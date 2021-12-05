using System.Net.Http;
using Newtonsoft.Json;
using Polly;
using ServiceStack.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;
using ValorDolarHoy.Common.Storage;
using ValorDolarHoy.Services;
using ValorDolarHoy.Services.Clients;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionsExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<BluelyticsService>();
            services.AddSingleton<IKvsStore, KvsStore>();

            services.AddSingleton<IRedisClientsManager, PooledRedisClientManager>(_ =>
                new PooledRedisClientManager("402639d6804af2a7bce70236e2ec3240@pike.redistogo.com:10753"));
        }

        public static void AddClients(this IServiceCollection services)
        {
            services.AddHttpClient<IBluelyticsClient, BluelyticsClient>()
                .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
                {
                    MaxConnectionsPerServer = 20
                })
                .AddPolicyHandler(Policy.BulkheadAsync<HttpResponseMessage>(20, int.MaxValue));
        }
    }
}