/* 
 * SchemaExtensions.cs
 * 
 *   Created: Sometime in 2016
 *   Modified: 2023-04-03-09:12:26
 * 
 *   Author: Nick Hauensteiin <Nicholas.Hauenstein@microsoft.com>
 *   Contributors: David G. Moore, Jr. david@dgmjr.io
 *                 CodeGPT (no really, it wrote a lot of this)
 *   
 *   Copyright © 2016 - 2023 Nick Hauenstein & David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */ 

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using TRex;
using TRex.Metadata;
using TRex.Metadata.Models;

namespace QuickLearn.ApiApps.Metadata.Extensions
{
    internal static class SchemaExtensions
    {
        public static void EnsureVendorExtensions(this OpenApiSchema modelDescription)
        {
            if (modelDescription.Extensions == null) modelDescription.Extensions = new Dictionary<string, IOpenApiExtension>();
        }

        public static void SetSchemaLookup(this OpenApiSchema modelDescription, DynamicSchemaModel dynamicSchemaSettings)
        {
            modelDescription.EnsureVendorExtensions();

            if (!modelDescription.Extensions.ContainsKey(Constants.X_MS_DYNAMIC_SCHEMA))
            {
                modelDescription.Extensions.Add(Constants.X_MS_DYNAMIC_SCHEMA, dynamicSchemaSettings);
            }
        }

        public static void SetValueLookup(this OpenApiSchema modelDescription, DynamicValuesModel dynamicValuesSettings)
        {

            modelDescription.EnsureVendorExtensions();

            if (!modelDescription.Extensions.ContainsKey(Constants.X_MS_DYNAMIC_VALUES))
            {
                modelDescription.Extensions.Add(Constants.X_MS_DYNAMIC_VALUES, dynamicValuesSettings);
            }

        }

        public static void SetCallbackUrl(this OpenApiSchema modelDescription)
        {
            modelDescription.EnsureVendorExtensions();

            if (!modelDescription.Extensions.ContainsKey(Constants.X_MS_NOTIFICATION_URL))
            {
                modelDescription.Extensions.Add(Constants.X_MS_NOTIFICATION_URL, true.ToString().ToLowerInvariant());
            }

            modelDescription.SetVisibility(VisibilityType.Internal);
            // todo: #3 Find out if this is stil necessary
            // modelDescription.Type = VisibilityType.Internal;
        }

        public static void SetVisibility(this OpenApiSchema modelDescription, VisibilityType visibility)
        {
            if (visibility == VisibilityType.Default) return;

            modelDescription.EnsureVendorExtensions();

            if (!modelDescription.Extensions.ContainsKey(Constants.X_MS_VISIBILITY))
            {
                modelDescription.Extensions
                    .Add(Constants.X_MS_VISIBILITY,  visibility.ToString().ToLowerInvariant());
            }
        }

        public static void SetFriendlyNameAndDescription(this OpenApiSchema modelDescription, string friendlyName, string description)
        {
            if (!string.IsNullOrWhiteSpace(description))
                modelDescription.Description = description;

            if (string.IsNullOrWhiteSpace(friendlyName)) return;

            modelDescription.EnsureVendorExtensions();

            if (!modelDescription.Extensions.ContainsKey(Constants.X_MS_SUMMARY))
            {
                modelDescription.Extensions.Add(Constants.X_MS_SUMMARY, friendlyName);
            }
        }

    }
}
