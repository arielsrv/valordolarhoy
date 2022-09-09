using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using ServiceStack;

#pragma warning disable CS1591

namespace ValorDolarHoy.Core.Common.Extensions;

public static class IHttpClientBuilderExtensions
{
    public static void WithAppSettings<TImplementation>(this IHttpClientBuilder httpClientBuilder,
        IConfiguration configuration)
    {
        string key = $"{typeof(TImplementation).Name.ToLowerInvariant()}:";

        int timeout = GetIntValue(configuration, key, "timeout");
        int maxConnectionsPerServer = GetIntValue(configuration, key, "max_connections");
        int maxParallelization = GetIntValue(configuration, key, "max_parallelization");

        httpClientBuilder
            .SetTimeout(timeout > 0 ? TimeSpan.FromMilliseconds(timeout) : TimeSpan.FromMilliseconds(5000))
            .SetMaxConnectionsPerServer(maxConnectionsPerServer > 0 ? maxConnectionsPerServer : 20)
            .SetMaxParallelization(maxParallelization > 0 ? maxParallelization : 20);
    }

    private static int GetIntValue(IConfiguration configuration, string key, string value)
    {
        return configuration[key + value].ToInt();
    }

    private static IHttpClientBuilder SetTimeout(this IHttpClientBuilder httpClientBuilder, TimeSpan timeSpan)
    {
        httpClientBuilder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(timeSpan));

        return httpClientBuilder;
    }

    private static IHttpClientBuilder SetMaxConnectionsPerServer(this IHttpClientBuilder httpClientBuilder,
        int maxConnectionsPerServer)
    {
        httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            MaxConnectionsPerServer = maxConnectionsPerServer
        });

        return httpClientBuilder;
    }

    private static void SetMaxParallelization(this IHttpClientBuilder httpClientBuilder,
        int maxParallelization)
    {
        httpClientBuilder.AddPolicyHandler(Policy.BulkheadAsync<HttpResponseMessage>(maxParallelization, int.MaxValue));
    }
}