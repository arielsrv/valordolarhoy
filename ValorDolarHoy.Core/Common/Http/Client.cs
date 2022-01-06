using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ValorDolarHoy.Core.Common.Exceptions;

namespace ValorDolarHoy.Core.Common.Http;

public class Client : HttpClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger<Client> logger;

    protected Client(HttpClient httpClient, ILogger<Client> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    protected IObservable<T> Get<T>(string requestUri) where T : new()
    {
        return Observable.Create(async (IObserver<T> observer) =>
        {
            HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, requestUri);
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            try
            {
                using HttpResponseMessage httpResponseMessage =
                    await this.httpClient.SendAsync(httpRequestMessage, CancellationToken.None);
                string response = await httpResponseMessage.Content.ReadAsStringAsync();

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    this.logger.LogError(
                        $"Request failed with uri {requestUri}. Status code: {(int)httpResponseMessage.StatusCode}. Raw message: {response}. ");
                    throw httpResponseMessage.StatusCode switch
                    {
                        HttpStatusCode.NotFound => new ApiNotFoundException(response),
                        HttpStatusCode.BadRequest => new ApiBadRequestException(response),
                        _ => new ApiException(httpResponseMessage.ReasonPhrase ?? "Unknown reason")
                    };
                }

                T result = JsonConvert.DeserializeObject<T>(response) ?? new T();

                observer.OnNext(result);
                observer.OnCompleted();
            }
            catch (Exception e)
            {
                e.Data.Add("HttpClient", this.GetType().Name);
                observer.OnError(e);
                throw;
            }
        });
    }
}