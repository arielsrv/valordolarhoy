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

        protected Client(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Gets the specified request URI.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <returns></returns>
        public virtual async Task<T> Get<T>(string requestUri)
        {
            AsyncBulkheadPolicy<T> bulkhead = Policy.BulkheadAsync<T>(20, int.MaxValue);

            return await bulkhead.ExecuteAsync(async () =>
            {
                HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, requestUri);
                httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using HttpResponseMessage httpResponseMessage = await this.httpClient.SendAsync(httpRequestMessage);
                string response = await httpResponseMessage.Content.ReadAsStringAsync();

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    throw new ApplicationException(httpResponseMessage.ReasonPhrase);
                }

                return JsonConvert.DeserializeObject<T>(response);
            });
        }
    }
}