using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly;
using Polly.Bulkhead;

namespace ValorDolarHoy.Common
{
    public class Client : HttpClient
    {
        /// <summary>
        /// The HTTP client
        /// </summary>
        private readonly HttpClient httpClient;

        public Client(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Gets the specified request URI.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="headers">The headers.</param>
        /// <returns></returns>
        protected async Task<T> Get<T>(string requestUri, IDictionary<string, string> headers = null)
        {
            AsyncBulkheadPolicy<T> bulkhead = Policy.BulkheadAsync<T>(20, int.MaxValue);

            return await bulkhead.ExecuteAsync(async () =>
            {
                HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, requestUri);
                httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (httpRequestMessage.Content != null)
                {
                    AddHeaders(headers, httpRequestMessage.Content.Headers);
                }

                using HttpResponseMessage httpResponseMessage = await this.httpClient.SendAsync(httpRequestMessage);
                string response = await httpResponseMessage.Content.ReadAsStringAsync();

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    throw new ApplicationException(httpResponseMessage.ReasonPhrase);
                }

                return JsonConvert.DeserializeObject<T>(response);
            });
        }

        /// <summary>
        /// Adds the headers.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="httpContentHeaders">The HTTP content headers.</param>
        private static void AddHeaders(IDictionary<string, string> headers, HttpHeaders httpContentHeaders)
        {
            if (headers is not { Count: > 0 }) return;
            foreach ((string key, string value) in headers)
            {
                httpContentHeaders.Add(key, value);
            }
        }
    }
}