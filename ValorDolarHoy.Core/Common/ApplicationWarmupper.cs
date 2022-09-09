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
    private readonly IApplicationWarmUpper applicationWarmUpper;

    public WarmupExecutor(IApplicationWarmUpper applicationWarmUpper)
    {
        this.applicationWarmUpper = applicationWarmUpper;
    }

    public static bool Initialized { get; private set; }

    public void Warmup()
    {
        Initialized = this.applicationWarmUpper.Warmup();
    }
}

public class ApplicationWarmupper : IApplicationWarmUpper
{
    private readonly string? baseUrl;

    private readonly HttpClient httpClient = new()
    {
        Timeout = TimeSpan.FromMilliseconds(5000)
    };

    private readonly int retries;
    private readonly TimeSpan wait;

    public ApplicationWarmupper(TimeSpan wait, int retries)
    {
        this.wait = wait;
        this.retries = retries;
        this.baseUrl = Environment
            .GetEnvironmentVariable("ASPNETCORE_URLS")?
            .Split(";")
            .FirstOrDefault();
    }

    public bool Warmup()
    {
        Thread.Sleep(this.wait);

        for (int i = 0; i < this.retries; i++)
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

            Thread.Sleep(this.wait);
        }

        return false;
    }
}