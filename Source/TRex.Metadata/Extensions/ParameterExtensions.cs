/* 
 * ParameterExtensions.cs
 * 
 *   Created: Sometime in 2016
 *   Modified: 2023-04-03-09:10:12
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

namespace QuickLearn.ApiApps.Metadata.Extensions
{
    public static class ParameterExtensions
    {
        public static void EnsureExtensions(this OpenApiParameter item)
        {
            if (item.Extensions == null) item.Extensions = new Dictionary<string, IOpenApiExtension>();
        }

        public static void SetVisibility(this OpenApiParameter parameter, VisibilityType visibility)
        {
            if (visibility == VisibilityType.Default) return;

            parameter.EnsureExtensions();

            if (!parameter.Extensions.ContainsKey(Constants.X_MS_VISIBILITY))
            {
                parameter.Extensions.Add(Constants.X_MS_VISIBILITY,
                    new DynamicOpenApiExtension(Constants.X_MS_VISIBILITY, visibility.ToString().ToLowerInvariant()));
            }
        }

        public static void SetValueLookup(this OpenApiParameter parameter, object dynamicValuesSettings)
        {

            parameter.EnsureExtensions();

            if (!parameter.Extensions.ContainsKey(Constants.X_MS_DYNAMIC_VALUES))
            {
                parameter.Extensions.Add(Constants.X_MS_DYNAMIC_VALUES,
                    new DynamicOpenApiExtension(Constants.X_MS_DYNAMIC_VALUES, dynamicValuesSetting));
            }
        }


        public static void SetFriendlyNameAndDescription(this OpenApiParameter parameter, string friendlyName, string description)
        {
            if (!string.IsNullOrWhiteSpace(description))
                parameter.Description = description;

            if (string.IsNullOrWhiteSpace(friendlyName)) return;

            parameter.EnsureExtensions();

            if (!parameter.Extensions.ContainsKey(Constants.X_MS_SUMMARY))
            {
                parameter.Extensions.Add(Constants.X_MS_SUMMARY, new DynamicOpenApiExtension(Constants.X_MS_SUMMARY, friendlyName));
            }
        }

    }
}
