using AutoMapper;
using Microsoft.Extensions.Logging;
using ValorDolarHoy.Mappings;
using Xunit;

namespace ValorDolarHoy.Test.Unit.Common.Mappings;

public class MappingsTest
{
    [Fact]
    public void MappingProfile_Verify_Mappings()
    {
        LoggerFactory loggerFactory = new();
        MapperConfigurationExpression config = new()
        {
            LicenseKey = "DEMO-LICENSE-KEY-FOR-TESTING"
        };
        config.AddProfile(new MappingProfile());
        MapperConfiguration mapperConfiguration = new(config, loggerFactory);
        mapperConfiguration.AssertConfigurationIsValid();
    }
}