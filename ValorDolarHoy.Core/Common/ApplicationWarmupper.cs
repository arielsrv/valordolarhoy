using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

#pragma warning disable CS1591

namespace ValorDolarHoy.Core.Common.Extensions;

public interface IApplicationWarmUpper
{
    public bool Warmup();
}

public class WarmupExecutor
{
    /// <summary>
    ///     Check flag
    /// </summary>
    private static bool Initialized { get; set; } = true;

    public static void Warmup(IApplicationWarmUpper applicationWarmUpper)
    {
        Initialized = applicationWarmUpper.Warmup();
    }
}

public class ApplicationWarmupper : IApplicationWarmUpper
{
    private readonly string? baseUrl = Environment
        .GetEnvironmentVariable("ASPNETCORE_URLS")?
        .Split(";")
        .FirstOrDefault();

    private readonly HttpClient httpClient = new()
    {
        Timeout = TimeSpan.FromMilliseconds(5000)
    };

    public bool Warmup()
    {
        Thread.Sleep(1000);

        for (var i = 0; i < 3; i++)
        {
            try
            {
                if (string.IsNullOrEmpty(this.baseUrl)) return true;

                var requestUri = $"{this.baseUrl}/Currency";
                HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, requestUri);
                HttpResponseMessage httpResponseMessage = this.httpClient.Send(httpRequestMessage);

                if (httpResponseMessage.StatusCode == HttpStatusCode.OK) return true;
            }
            catch (Exception)
            {
                // ignored
            }

            Thread.Sleep(1000);
        }

        return false;
    }
}