using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Threading;
using Newtonsoft.Json;
using ValorDolarHoy.Common.Exceptions;

namespace ValorDolarHoy.Common
{
    public class Client : HttpClient
    {
        private readonly HttpClient httpClient;

        protected Client(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        protected IObservable<T> Get<T>(string requestUri)
        {
            return Observable.Create(async (IObserver<T> observer) =>
            {
                HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, requestUri);
                httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using HttpResponseMessage httpResponseMessage =
                    await this.httpClient.SendAsync(httpRequestMessage, CancellationToken.None);
                string response = await httpResponseMessage.Content.ReadAsStringAsync();

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    throw httpResponseMessage.StatusCode switch
                    {
                        HttpStatusCode.NotFound => new ApiNotFoundException("Not found: " + requestUri),
                        HttpStatusCode.BadRequest => new ApiBadRequestException("Not found: " + requestUri),
                        _ => new ApiException(httpResponseMessage.ReasonPhrase)
                    };
                }

                T result = JsonConvert.DeserializeObject<T>(response);

                observer.OnNext(result);
                observer.OnCompleted();
            });
        }
    }
}