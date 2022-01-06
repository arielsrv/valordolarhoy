using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace ValorDolarHoy.Core.Extensions;

public static class IHttpClientBuilderExtensions
{
    public static IHttpClientBuilder SetTimeout(this IHttpClientBuilder httpClientBuilder, TimeSpan timeSpan)
    {
        httpClientBuilder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(timeSpan));
        
        return httpClientBuilder;
    }

    public static IHttpClientBuilder SetMaxConnectionsPerServer(this IHttpClientBuilder httpClientBuilder,
        int maxConnectionsPerServer)
    {
        httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            MaxConnectionsPerServer = maxConnectionsPerServer
        });

        return httpClientBuilder;
    }

    public static IHttpClientBuilder SetMaxParallelization(this IHttpClientBuilder httpClientBuilder,
        int maxParallelization)
    {
        httpClientBuilder.AddPolicyHandler(Policy.BulkheadAsync<HttpResponseMessage>(maxParallelization, int.MaxValue));

        return httpClientBuilder;
    }
}