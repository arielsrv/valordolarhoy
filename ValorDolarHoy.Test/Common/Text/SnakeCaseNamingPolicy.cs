using ValorDolarHoy.Core.Common.Text;
using Xunit;

namespace ValorDolarHoy.Test.Common.Text;

public class SnakeCaseNamingPolicyTest
{
    [Fact]
    public void Convert_To_Name()
    {
        SnakeCaseNamingPolicy snakeCaseNamingPolicy = new();

        string actual = snakeCaseNamingPolicy.ConvertName("HelloWorld");

        Assert.Equal("hello_world", actual);
    }
}