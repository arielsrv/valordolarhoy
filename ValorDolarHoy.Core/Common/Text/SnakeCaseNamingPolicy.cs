using System.Text.Json;
using ValorDolarHoy.Core.Common.Extensions;

#pragma warning disable CS1591

namespace ValorDolarHoy.Core.Common.Text;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.ToSnakeCase();
    }
}
