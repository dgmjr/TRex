/* 
 * TRexDocumentFilter.cs
 * 
 *   Created: Sometime in 2016
 *   Modified: 2023-04-03-09:10:33
 * 
 *   Author: Nick Hauensteiin <Nicholas.Hauenstein@microsoft.com>
 *   Contributors: David G. Moore, Jr. david@dgmjr.io
 *                 CodeGPT (no really, it wrote a lot of this)
 *   
 *   Copyright © 2016 - 2023 Nick Hauenstein & David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */ 

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using QuickLearn.ApiApps.Metadata;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TRex.Metadata
{
    internal class TRexDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (swaggerDoc == null) return;

            // This iterates through the paths and "moves up" any x-ms-notification-content
            // vendor extension values to the path level where the designer expects them to live
            foreach (var path in swaggerDoc.Paths)
            {
                var operations = path.Value.Operations;
                if (operations != null)
                {
                    foreach (var operation in operations)
                    {
                        var currentOperation = operation.Value;
                        if (currentOperation == null) continue;

                        if (currentOperation.Extensions != null &&
                            currentOperation.Extensions.ContainsKey(Constants.X_MS_NOTIFICATION_CONTENT))
                        {
                            var extensions = currentOperation.Extensions;
                            var notificationContent = extensions[Constants.X_MS_NOTIFICATION_CONTENT];

                            if (path.Value.Extensions == null)
                            {
                                path.Value.Extensions = new Dictionary<string, IOpenApiExtension>();
                            }
                            path.Value.Extensions[Constants.X_MS_NOTIFICATION_CONTENT] = notificationContent;

                            extensions.Remove(Constants.X_MS_NOTIFICATION_CONTENT);
                        }
                    }
                }
            }
        }
    }
}
