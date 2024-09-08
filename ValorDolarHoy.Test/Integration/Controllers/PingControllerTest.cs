using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace ValorDolarHoy.Test.Integration.Controllers;

public class PingControllerTest
{
    private readonly HttpClient httpClient;

    public PingControllerTest()
    {
        IHostBuilder hostBuilder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.UseStartup<Startup>();
            });

        IHost host = hostBuilder.Start();

        this.httpClient = host.GetTestClient();
    }

    [Fact]
    public async Task PingAsync()
    {
        HttpResponseMessage httpResponseMessage = await this.httpClient.GetAsync("/ping");
        var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("pong", responseString);
    }
}
