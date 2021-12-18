using System.Text.Json;
using ValorDolayHoy.Core.Common.Extensions;

namespace ValorDolayHoy.Core.Common.Text;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.ToSnakeCase();
    }
}