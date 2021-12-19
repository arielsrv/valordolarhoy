using Newtonsoft.Json.Serialization;
using System;

namespace ValorDolarHoy.Core.Common.Extensions;

public static class StringExtensions
{
    private static readonly SnakeCaseNamingStrategy snakeCaseNamingStrategy = new();

    public static string ToSnakeCase(this string? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return snakeCaseNamingStrategy.GetPropertyName(value, false);
    }
}