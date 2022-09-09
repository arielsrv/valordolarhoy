using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace ValorDolarHoy.Core.Common.Extensions;

public interface IApplicationWarmUpper
{
    public bool Warmup();
}

public class WarmupExecutor
{
    public static bool Initialized { get; private set; }

    public static void Warmup(IApplicationWarmUpper applicationWarmUpper)
    {
        Initialized = applicationWarmUpper.Warmup();
    }
}

public class ApplicationWarmupper : IApplicationWarmUpper
{
    private readonly string? baseUrl;

    private readonly HttpClient httpClient = new()
    {
        Timeout = TimeSpan.FromMilliseconds(5000)
    };

    public ApplicationWarmupper()
    {
        this.baseUrl = Environment
            .GetEnvironmentVariable("ASPNETCORE_URLS")?
            .Split(";")
            .FirstOrDefault();
    }

    public bool Warmup()
    {
        Thread.Sleep(1000);

        for (int i = 0; i < 3; i++)
        {
            try
            {
                if (string.IsNullOrEmpty(this.baseUrl)) return true;

                string requestUri = $"{this.baseUrl}/Currency";
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