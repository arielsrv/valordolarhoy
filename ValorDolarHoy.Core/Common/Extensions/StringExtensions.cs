using System;
using Ardalis.GuardClauses;
using Newtonsoft.Json.Serialization;

#pragma warning disable CS1591

namespace ValorDolarHoy.Core.Common.Extensions;

public static class StringExtensions
{
    private static readonly SnakeCaseNamingStrategy snakeCaseNamingStrategy = new();

    public static string ToSnakeCase(this string? value)
    {
        Guard.Against.NullOrEmpty(value);

        return snakeCaseNamingStrategy.GetPropertyName(value, false);
    }
}