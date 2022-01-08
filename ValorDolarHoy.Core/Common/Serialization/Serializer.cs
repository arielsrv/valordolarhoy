using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ValorDolarHoy.Core.Common.Text;

namespace ValorDolarHoy.Core.Common.Serialization;

public static class Serializer
{
    public static void BuildSettings(JsonOptions jsonOptions)
    {
        BuildIncomingSettings();
        BuildOutgoingSettings(jsonOptions);
    }

    public static void BuildIncomingSettings()
    {
        JsonConvert.DefaultSettings = () =>
        {
            JsonSerializerSettings jsonSerializerSettings = new()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            jsonSerializerSettings.Converters.Add(new StringEnumConverter());

            return jsonSerializerSettings;
        };
    }

    private static void BuildOutgoingSettings(JsonOptions jsonOptions)
    {
        jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
    }
}