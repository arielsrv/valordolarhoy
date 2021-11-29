using System.Net.Http;
using Polly;
using ValorDolarHoy.Services;
using ValorDolarHoy.Services.Clients;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionsExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<BluelyticsService>();
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