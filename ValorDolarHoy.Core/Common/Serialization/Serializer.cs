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
}