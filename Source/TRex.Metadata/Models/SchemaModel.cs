/* 
 * SchemaModel.cs
 * 
 *   Created: Sometime in 2016
 *   Modified: 2023-04-03-09:14:20
 * 
 *   Author: Nick Hauensteiin <Nicholas.Hauenstein@microsoft.com>
 *   Contributors: David G. Moore, Jr. david@dgmjr.io
 *                 CodeGPT (no really, it wrote a lot of this)
 *   
 *   Copyright © 2016 - 2023 Nick Hauenstein & David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */ 

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
