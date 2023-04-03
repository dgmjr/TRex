using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using QuickLearn.ApiApps.Metadata;

namespace TRex.Metadata.Models
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SchemaModel
    {
        public SchemaModel(OpenApiSchema schema)
        {
            if (schema.Reference != null)
                Reference = schema.Reference;


            if (schema.Type != null)
                Type = schema.Type;
        }

        [JsonProperty(PropertyName = Constants.TYPE, NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(PropertyName = Constants.REF, NullValueHandling = NullValueHandling.Ignore)]
        public OpenApiReference Reference { get; set; }
    }
}
