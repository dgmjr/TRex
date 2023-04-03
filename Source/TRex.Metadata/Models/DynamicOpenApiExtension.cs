/* 
 * DynamicOpenApiExtension.cs
 * 
 *   Created: Sometime in 2016
 *   Modified: 2023-04-03-09:13:08
 * 
 *   Author: Nick Hauensteiin <Nicholas.Hauenstein@microsoft.com>
 *   Contributors: David G. Moore, Jr. david@dgmjr.io
 *                 CodeGPT (no really, it wrote a lot of this)
 *   
 *   Copyright © 2016 - 2023 Nick Hauenstein & David G. Moore, Jr., All Rights Reserved
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
