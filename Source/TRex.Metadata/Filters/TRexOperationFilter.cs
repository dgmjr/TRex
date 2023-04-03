/* 
 * TRexOperationFilter.cs
 * 
 *   Created: Sometime in 2016
 *   Modified: 2023-04-03-09:12:44
 * 
 *   Author: Nick Hauensteiin <Nicholas.Hauenstein@microsoft.com>
 *   Contributors: David G. Moore, Jr. david@dgmjr.io
 *                 CodeGPT (no really, it wrote a lot of this)
 *   
 *   Copyright © 2016 - 2023 Nick Hauenstein & David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */ 

namespace QuickLearn.ApiApps.Metadata
{
    using System.Linq;
    using System.Reflection;
    using Microsoft.OpenApi.Models;
    using QuickLearn.ApiApps.Metadata.Extensions;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using TRex.Metadata;
    using TRex.Metadata.Models;

    internal class TRexOperationFilter : IOperationFilter
    {

        public TRexOperationFilter() { }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            if (operation == null) return;

            // Handle Metadata attribute
            applyOperationMetadataAndVisibility(operation, context);

            // Handle DynamicValueLookup attribute
            applyValueLookupForDynamicParameters(operation, context);

            // Handle Trigger attribute
            applyTriggerBatchModeAndResponse(operation, context);

            // Apply default response (copy 200 level response if available as "default")
            applyDefaultResponse(operation);

        }

        private static void applyTriggerBatchModeAndResponse(OpenApiOperation operation, OperationFilterContext context)
        {
            var triggerInfo = context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<TriggerAttribute>().FirstOrDefault();

            if (triggerInfo == null) return;

            operation.SetTrigger(context.SchemaGenerator, triggerInfo);
        }

        private static void applyValueLookupForDynamicParameters(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null || context == null) return;

            var lookupParameters = from p in context.ApiDescription.ParameterDescriptions
                                    let valueLookupInfo = p.CustomAttributes().OfType<DynamicValueLookupAttribute>().FirstOrDefault()
                                    where valueLookupInfo != null
                                    select new
                                    {
                                        SwaggerParameter = operation.Parameters.FirstOrDefault(param => param.Name == p.Name),
                                        Parameter = p,
                                        ValueLookupInfo = valueLookupInfo
                                    };

            if (!lookupParameters.Any()) return;

            foreach (var param in lookupParameters)
            {
                var valueLookup = new DynamicValuesModel()
                {
                    Parameters = ParsingUtility.ParseJsonOrUrlEncodedParams(param.ValueLookupInfo.Parameters),
                    ValueCollection = param.ValueLookupInfo.ValueCollection,
                    ValuePath = param.ValueLookupInfo.ValuePath,
                    ValueTitle = param.ValueLookupInfo.ValueTitle
                };

                valueLookup.OperationId =
                    context.ApiDescription.ResolveOperationIdForSiblingAction(
                        param.ValueLookupInfo.LookupOperation,
                        valueLookup.Parameters.Properties().Select(p => p.Name).ToArray());

                param.SwaggerParameter.SetValueLookup(valueLookup);
            }
        }

        /// <summary>
        /// Applies the friendly names, descriptions, and visibility settings to the operation
        /// </summary>
        /// <param name="operation">Exposed operation metadata</param>
        /// <param name="context">Implementation metadata</param>
        private static void applyOperationMetadataAndVisibility(OpenApiOperation operation, OperationFilterContext context)
        {

            // Apply friendly names and descriptions where possible
            //      operation.summary - friendly name (applies to methods)
            //      operation.description - description (applies to methods)
            //      "x-ms-summary" - friendly name (applies to parameters and their properties)
            //      operation.parameters[x].description - description (applies to parameters)

            var operationMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<MetadataAttribute>().FirstOrDefault();

            if (operationMetadata != null)
            {
                operation.SetFriendlyNameAndDescription(operationMetadata.FriendlyName, operationMetadata.Description);
                operation.SetVisibility(operationMetadata.Visibility);
            }

            if (operation.Parameters == null) return;

            // Ensure that we get the parameters of the operation all annotated appropriately as well
            applyOperationParameterMetadataAndVisibility(operation, context);

        }

        /// <summary>
        /// Applies the friendly names, descriptions, and visibility settings to all operation parameters
        /// </summary>
        /// <param name="operation">Exposed operation metadata</param>
        /// <param name="context">Implementation metadata</param>
        private static void applyOperationParameterMetadataAndVisibility(OpenApiOperation operation, OperationFilterContext context)
        {
            var operationParameters = context.ApiDescription.ActionDescriptor.Parameters;

            if (operationParameters != null)
            {
                foreach (var parameter in context.ApiDescription.ActionDescriptor.Parameters)
                {
                    var parameterMetadata = parameter.ParameterType.GetCustomAttributes<MetadataAttribute>().FirstOrDefault();

                    var operationParam = operation.Parameters.FirstOrDefault(p => p.Name == parameter.Name);

                    if (operationParam != null && parameterMetadata != null)
                    {
                        operationParam.SetFriendlyNameAndDescription(parameterMetadata.FriendlyName, parameterMetadata.Description);
                        operationParam.SetVisibility(parameterMetadata.Visibility);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that each operation has a "default" response with a 200-level response code
        /// </summary>
        /// <param name="operation">Metadata for the operation</param>
        private static void applyDefaultResponse(OpenApiOperation operation)
        {
            if (!operation.Responses.ContainsKey(Constants.DEFAULT_RESPONSE_KEY))
            {
                var successCode = (from statusCode in operation.Responses.Keys
                                            where statusCode.StartsWith(Constants.HAPPY_RESPONSE_CODE_LEVEL_START,
                                                System.StringComparison.OrdinalIgnoreCase)
                                            select statusCode).FirstOrDefault();

                if (successCode != null)
                {
                    operation.Responses.Add(Constants.DEFAULT_RESPONSE_KEY, operation.Responses[successCode]);
                }
            }
        }

    }
}
