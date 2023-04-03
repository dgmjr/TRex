/* 
 * DynamicOpenApiExtension.cs
 * 
 *   Created: 2023-04-03-03:38:47
 *   Modified: 2023-04-03-03:38:47
 * 
 *   Author: Justin Chase <justin@justinwritescode.com>
 *   
 *   Copyright © 2022-2023 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace TRex;

public class DynamicOpenApiExtension : IOpenApiExtension
{
    public string Name { get; set; }
    public IOpenApiAny Value { get; set; }

    public DynamicOpenApiExtension(string name, IOpenApiAny value)
    {
        Name = name;
        Value = value;
    }
    public DynamicOpenApiExtension(string name, string value)
    {
        Name = name;
        Value = new OpenApiString(value);
    }

    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        writer.WritePropertyName(Name);
        Value.Write(writer, specVersion);
    }
}
