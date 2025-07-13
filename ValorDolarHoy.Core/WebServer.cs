using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

#pragma warning disable CS1591

namespace ValorDolarHoy;

public static class WebServer
{
    public static void Run(string[] args)
    {
        CreateHostBuilder(args)
            .Build()
            .Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}