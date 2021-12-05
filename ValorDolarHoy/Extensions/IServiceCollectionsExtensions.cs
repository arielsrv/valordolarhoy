using System.Net.Http;
using Newtonsoft.Json;
using Polly;
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

            services.AddSingleton<IRedisCacheClient, RedisCacheClient>(_ =>
            {
                RedisConfiguration redisConfiguration = new()
                {
                    ConnectionString =
                        "localhost:6379"
                };

                IRedisCacheConnectionPoolManager redisCacheConnectionPoolManager =
                    new RedisCacheConnectionPoolManager(redisConfiguration);

                NewtonsoftSerializer newtonsoftSerializer = new(new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    NullValueHandling = NullValueHandling.Include
                });

                return new RedisCacheClient(redisCacheConnectionPoolManager, newtonsoftSerializer, redisConfiguration);
            });
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