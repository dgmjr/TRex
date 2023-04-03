using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using TRex.Metadata;

namespace QuickLearn.ApiApps.Metadata.Extensions
{
    internal static class ApiDescriptionExtensions
    {

        internal static string ResolveOperationIdForSiblingAction(this ApiDescription apiDescription, string actionName, string[] parameterNames)
        {
            string operationId = actionName;

            var matchingMethod = (from method in apiDescription.ActionDescriptor.EndpointMetadata.SelectMany(m => m.GetType().GetMethods())
                                                            .Where(m => m.GetCustomAttributes(typeof(MetadataAttribute), true).Any())
                                            let parameters = method.GetParameters()
                                                            .Select(p => p.Name)
                                                            .OrderBy(p => p)
                                                            .ToArray()
                                            where method.Name == actionName
                                                    && parameters.SequenceEqual(parameterNames)
                                            select method).FirstOrDefault();

            if (matchingMethod != null)
            {

                var methodAttributes = matchingMethod.GetCustomAttributes(typeof(MetadataAttribute), true);

                MetadataAttribute methodMetadata = methodAttributes != null ? (MetadataAttribute)methodAttributes.FirstOrDefault() : null;

                if (methodMetadata != null)
                    operationId = OperationExtensions.GetOperationId(methodMetadata.FriendlyName);

            }

            return operationId;
        }

        // internal static T GetFirstOrDefaultCustomAttribute<T>(this ActionDescriptor actionDescriptor) where T : Attribute
        // {
        //     if (actionDescriptor == null) return null;

        //     var attributeInfoResult = actionDescriptor.GetFirstOrDefaultCustomAttribute<T>();

        //     return attributeInfoResult == null ? null : attributeInfoResult.FirstOrDefault();
        // }

        // internal static T GetFirstOrDefaultCustomAttribute<T>(this HttpParameterDescriptor parameterDescriptor) where T : Attribute
        // {
        //     if (parameterDescriptor == null) return null;

        //     var attributeInfoResult = parameterDescriptor.GetCustomAttributes<T>();
        //     return attributeInfoResult == null ? null : attributeInfoResult.FirstOrDefault();
        // }
    }
}
