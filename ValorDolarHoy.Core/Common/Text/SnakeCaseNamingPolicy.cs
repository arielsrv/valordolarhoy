using System.Text.Json;
using ValorDolarHoy.Core.Common.Extensions;

namespace ValorDolarHoy.Core.Common.Text;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.ToSnakeCase();
    }
}