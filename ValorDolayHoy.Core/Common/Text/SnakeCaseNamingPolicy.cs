using System;
using System.Text.Json;

namespace ValorDolarHoy.Common.Text;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.ToSnakeCase();
    }
}