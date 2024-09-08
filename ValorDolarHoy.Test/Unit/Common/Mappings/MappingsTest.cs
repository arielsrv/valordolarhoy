using AutoMapper;
using ValorDolarHoy.Mappings;
using Xunit;

namespace ValorDolarHoy.Test.Unit.Common.Mappings;

public class MappingsTest
{
    [Fact]
    public void MappingProfile_Verify_Mappings()
    {
        MapperConfiguration mapperConfiguration = new(configure => { configure.AddProfile(new MappingProfile()); });
        mapperConfiguration.AssertConfigurationIsValid();
    }
}
