using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ValorDolarHoy.Core.Common.Serialization;
using ValorDolarHoy.Core.Extensions;

namespace ValorDolarHoy;

public class Application : Startup
{
    public Application(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void Init(IServiceCollection services)
    {
        services.AddClients();
        services.AddMappings();
        services.AddServices(this.configuration);

        services
            .AddMvc()
            .AddJsonOptions(Serializer.BuildSettings);
    }
}