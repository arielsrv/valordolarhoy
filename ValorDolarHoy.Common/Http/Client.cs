using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ValorDolarHoy.Common.Exceptions;

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
        protected async Task<T> GetAsync<T>(string requestUri)
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
                    _ => new ApiException(httpResponseMessage.ReasonPhrase)
                };
            }

            return JsonConvert.DeserializeObject<T>(response);
        }
    }
}