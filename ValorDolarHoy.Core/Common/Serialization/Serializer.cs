using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ValorDolarHoy.Core.Common.Serialization;

public static class Serializer
{
    public static void JsonSerializerSettings()
    {
        JsonConvert.DefaultSettings = () =>
        {
            JsonSerializerSettings jsonSerializerSettings = new();

            jsonSerializerSettings.Converters.Add(new StringEnumConverter());
            jsonSerializerSettings.Formatting = Formatting.Indented;
            jsonSerializerSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            return jsonSerializerSettings;
        };
    }
}