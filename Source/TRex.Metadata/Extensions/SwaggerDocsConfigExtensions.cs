/* 
 * SwaggerDocsConfigExtensions.cs
 * 
 *   Created: Sometime in 2016
 *   Modified: 2023-04-03-09:10:51
 * 
 *   Author: Nick Hauensteiin <Nicholas.Hauenstein@microsoft.com>
 *   Contributors: David G. Moore, Jr. david@dgmjr.io
 *                 CodeGPT (no really, it wrote a lot of this)
 *   
 *   Copyright © 2016 - 2023 Nick Hauenstein & David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

using System;

using Microsoft.Extensions.DependencyInjection;

using QuickLearn.ApiApps.Metadata;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace TRex.Metadata
{
    public static class SwaggerDocsConfigExtensions
    {
        /// <summary>
        /// Makes your API App more easily consumable in the Logic App designer.
        /// Adds a schema filter and operation filter to apply changes declared
        /// using the T-Rex MetadataAttribute, TriggerAttribute, and
        /// the UnregisterCallbackAttribute.
        /// </summary>
        /// <param name="config">SwaggerDocsConfig instance that will be used
        /// to configure Swashbuckle</param>
        [CLSCompliant(false)]
        public static void ReleaseTheTRex(this SwaggerGenOptions config)
        {
            if (config == null) return;

            config.SchemaFilter<TRexSchemaFilter>();
            config.OperationFilter<TRexOperationFilter>();
            config.DocumentFilter<TRexDocumentFilter>();
        }
    }
}
