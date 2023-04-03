﻿/* 
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

using Swashbuckle.Swagger;
using System.Collections.Generic;
using TRex.Metadata;
using TRex.Metadata.Models;

namespace QuickLearn.ApiApps.Metadata.Extensions
{
    internal static class SchemaExtensions
    {
        public static void EnsureVendorExtensions(this Schema modelDescription)
        {
            if (modelDescription.vendorExtensions == null) modelDescription.vendorExtensions = new Dictionary<string, object>();
        }

        public static void SetSchemaLookup(this Schema modelDescription, DynamicSchemaModel dynamicSchemaSettings)
        {
            modelDescription.EnsureVendorExtensions();

            if (!modelDescription.vendorExtensions.ContainsKey(Constants.X_MS_DYNAMIC_SCHEMA))
            {
                modelDescription.vendorExtensions.Add(Constants.X_MS_DYNAMIC_SCHEMA,
                    dynamicSchemaSettings);
            }
        }

        public static void SetValueLookup(this Schema modelDescription, DynamicValuesModel dynamicValuesSettings)
        {

            modelDescription.EnsureVendorExtensions();

            if (!modelDescription.vendorExtensions.ContainsKey(Constants.X_MS_DYNAMIC_VALUES))
            {
                modelDescription.vendorExtensions.Add(Constants.X_MS_DYNAMIC_VALUES,
                    dynamicValuesSettings);
            }

        }

        public static void SetCallbackUrl(this Schema modelDescription)
        {
            modelDescription.EnsureVendorExtensions();

            if (!modelDescription.vendorExtensions.ContainsKey(Constants.X_MS_NOTIFICATION_URL))
            {
                modelDescription.vendorExtensions.Add(Constants.X_MS_NOTIFICATION_URL,
                    true.ToString().ToLowerInvariant());
            }

            modelDescription.SetVisibility(VisibilityType.Internal);
        }

        public static void SetVisibility(this Schema modelDescription, VisibilityType visibility)
        {
            if (visibility == VisibilityType.Default) return;

            modelDescription.EnsureVendorExtensions();

            if (!modelDescription.vendorExtensions.ContainsKey(Constants.X_MS_VISIBILITY))
            {
                modelDescription.vendorExtensions
                    .Add(Constants.X_MS_VISIBILITY,
                            visibility.ToString().ToLowerInvariant());
            }
        }

        public static void SetFriendlyNameAndDescription(this Schema modelDescription, string friendlyName, string description)
        {
            if (!string.IsNullOrWhiteSpace(description))
                modelDescription.description = description;

            if (string.IsNullOrWhiteSpace(friendlyName)) return;

            modelDescription.EnsureVendorExtensions();

            if (!modelDescription.vendorExtensions.ContainsKey(Constants.X_MS_SUMMARY))
            {
                modelDescription.vendorExtensions.Add(Constants.X_MS_SUMMARY, friendlyName);
            }
        }

    }
}
