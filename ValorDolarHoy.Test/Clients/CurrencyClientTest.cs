using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Moq;
using ValorDolarHoy.Core.Clients.Currency;
using ValorDolarHoy.Core.Common.Exceptions;
using ValorDolarHoy.Core.Common.Serialization;
using Xunit;

namespace ValorDolarHoy.Test.Clients;

public class CurrencyClientTest
{
    private readonly Mock<HttpClient> httpClient;
    private readonly Mock<ILogger<CurrencyClient>> logger;

    public CurrencyClientTest()
    {
        Serializer.JsonSerializerSettings();
        this.httpClient = new Mock<HttpClient>();
        this.logger = new Mock<ILogger<CurrencyClient>>();
    }

    [Fact]
    public void Get_Latest_Ok()
    {
        this.httpClient
            .Setup(client => client.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetResponse());

        CurrencyClient currencyClient = new(this.httpClient.Object, this.logger.Object);
        CurrencyResponse currencyResponse = currencyClient.Get().Wait();

        Assert.NotNull(currencyResponse);
        Assert.NotNull(currencyResponse.Oficial);
        Assert.Equal(105.96m, currencyResponse.Oficial!.ValueSell);
    }

    [Fact]
    public void Get_Latest_Not_Found()
    {
        this.httpClient
            .Setup(client => client.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        CurrencyClient currencyClient = new(this.httpClient.Object, this.logger.Object);

        Assert.Throws<ApiNotFoundException>(() => currencyClient.Get().Wait());
    }

    [Fact]
    public void Get_Latest_Bad_Request()
    {
        this.httpClient
            .Setup(client => client.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        CurrencyClient currencyClient = new(this.httpClient.Object, this.logger.Object);

        Assert.Throws<ApiBadRequestException>(() => currencyClient.Get().Wait());
    }

    [Fact]
    public void Get_Latest_Generic_Error()
    {
        this.httpClient
            .Setup(client => client.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        CurrencyClient currencyClient = new(this.httpClient.Object, this.logger.Object);

        Assert.Throws<ApiException>(() => currencyClient.Get().Wait());
    }

    private static HttpResponseMessage GetResponse()
    {
        return new HttpResponseMessage
        {
            Content = new StringContent(
                "{\"oficial\":{\"value_avg\":102.96,\"value_sell\":105.96,\"value_buy\":99.96},\"blue\":{\"value_avg\":199.50,\"value_sell\":201.50,\"value_buy\":197.50},\"oficial_euro\":{\"value_avg\":110.71,\"value_sell\":113.94,\"value_buy\":107.48},\"blue_euro\":{\"value_avg\":214.52,\"value_sell\":216.67,\"value_buy\":212.37},\"last_update\":\"2021-11-19T19:55:35.460166-03:00\"}")
        };
    }
}