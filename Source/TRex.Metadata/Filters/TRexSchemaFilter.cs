using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models;
using QuickLearn.ApiApps.Metadata.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerGen;
using TRex.Metadata;
using TRex.Metadata.Models;

namespace QuickLearn.ApiApps.Metadata
{
    internal class TRexSchemaFilter : ISchemaFilter
    {

        public TRexSchemaFilter()
        {

        }

        public void Apply(OpenApiSchema schema,  SchemaRepository schemaRegistry, Type type)
        {
            if (schema == null || type == null)
            {
                return;
            }

            applySchemaLookupForDynamicModels(schema, schemaRegistry, type);

            if (schema.Properties == null)
            {
                return;
            }

            foreach (var propertyName in schema.Properties.Keys)
            {
                var schemaProperty = schema.Properties[propertyName];
                var propertyInfo = type.GetRuntimeProperties().Where(p => p.GetSerializedPropertyName() == propertyName).FirstOrDefault();

                applyValueLookupForDynamicProperties(schemaProperty, propertyInfo);
                applyPropertyMetadata(schemaProperty, propertyInfo);
                applyCallbackUrl(schemaProperty, propertyInfo);
            }
        }

        private static void applyValueLookupForDynamicProperties(OpenApiSchema schemaProperty, PropertyInfo propertyInfo)
        {

            // Applies dynamic value lookup for the current property (if DynamicValueLookupAttribute is present)

            if (schemaProperty == null || propertyInfo == null) return;

            var valueLookupInfo = propertyInfo.GetCustomAttribute<DynamicValueLookupAttribute>();

            if (valueLookupInfo == null) return;

            var valueLookup = new DynamicValuesModel()
            {
                Parameters = ParsingUtility.ParseJsonOrUrlEncodedParams(valueLookupInfo.Parameters),
                ValueCollection = valueLookupInfo.ValueCollection,
                ValuePath = valueLookupInfo.ValuePath,
                ValueTitle = valueLookupInfo.ValueTitle
            };

            valueLookup.OperationId = valueLookupInfo.LookupOperation;

            schemaProperty.SetValueLookup(valueLookup);
        }

        private static void applySchemaLookupForDynamicModels(OpenApiSchema schema, SchemaRepository schemaRegistry, Type type)
        {
            if (schema == null || type == null) return;

            var dynamicSchemaInfo = type.GetCustomAttributes<DynamicSchemaLookupAttribute>().FirstOrDefault();

            if (null == dynamicSchemaInfo) return;

            var schemaLookupSettings = new DynamicSchemaModel()
            {
                OperationId = dynamicSchemaInfo.LookupOperation,
                Parameters = ParsingUtility.ParseJsonOrUrlEncodedParams(dynamicSchemaInfo.Parameters),
                ValuePath = dynamicSchemaInfo.ValuePath
            };

            // Swashbuckle should end up generating a ref schema in this case already,
            // in which case all that is necessary is to apply the vendor extensions and 
            // get out of this method
            if (type.BaseType == typeof(object))
            {
                // I don't know wtf this is supposed to do so I'm'a just comment it out for now
                // todo: figure out what this is supposed to do n fix it
                // schema.SetSchemaLookup(schemaLookupSettings);
                return;
            }

            // Determine if the dynamic schema already appears in the schema registry
            // if it appears, we will reference it's definition and make sure it has the
            // vendor extension applied 
            if (schemaRegistry.TryLookupByType(type, out var refSchema))
            {
                schemaRegistry.Schemas[type.Name] = refSchema;
                // schemaRegistry.Definitions[type.Name].SetSchemaLookup(schemaLookupSettings);
            }
            else
            {
                // Dynamic schema hasn't been registered yet, let's do that to make sure
                // the schema doesn't get inlined (since the settings will be common for the type
                // given that the attribute appears at the class-level)
                var dynamicSchema = new OpenApiSchema()
                {
                    AdditionalProperties = schema.AdditionalProperties,
                    AllOf = schema.AllOf,
                    Default = schema.Default,
                    Description = schema.Description,
                    Discriminator = schema.Discriminator,
                    Enum = schema.Enum,
                    Example = schema.Example,
                    ExclusiveMaximum = schema.ExclusiveMaximum,
                    ExclusiveMinimum = schema.ExclusiveMinimum,
                    ExternalDocs = schema.ExternalDocs,
                    Format = schema.Format,
                    Items = schema.Items,
                    Maximum = schema.Maximum,
                    MaxItems = schema.MaxItems,
                    MaxLength = schema.MaxLength,
                    MaxProperties = schema.MaxProperties,
                    Minimum = schema.Minimum,
                    MinItems = schema.MinItems,
                    MinLength = schema.MinLength,
                    MinProperties = schema.MinProperties,
                    MultipleOf = schema.MultipleOf,
                    Pattern = schema.Pattern,
                    Properties = schema.Properties,
                    ReadOnly = schema.ReadOnly,
                    Reference = schema.Reference,
                    Required = schema.Required,
                    Title = schema.Title,
                    Type = schema.Type,
                    UniqueItems = schema.UniqueItems,
                    Xml = schema.Xml
                };

                /**
                  * I don't know wtf this is supposed to do so I'm'a just comment it out for now
                // todo: figure out what this is supposed to do n fix it

                dynamicSchema.SetSchemaLookup(schemaLookupSettings);
                dynamicSchema.properties = dynamicSchema.properties ?? new Dictionary<string, Schema>();

                schemaRegistry.Definitions.Add(type.Name, dynamicSchema);
                 */

            }

            // Let's make sure the current schema points to the definition that is registered
            // and doesn't get inlined
            if (string.IsNullOrWhiteSpace(schema.Reference?.ToString()))
            {
                schema.Reference = new OpenApiReference { Id = type.Name, ExternalResource = null, Type = ReferenceType.Schema };
                // schema.Type = null;
                // schema.Properties = null;
            }

        }


        private static void applyCallbackUrl(OpenApiCallback schemaProperty, PropertyInfo propertyInfo)
        {

            if (schemaProperty == null || propertyInfo == null) return;

            var callbackUrlAttribute = propertyInfo.GetCustomAttribute<CallbackUrlAttribute>();

            if (callbackUrlAttribute != null)
            {
                // I also don't know wtf this is supposed to do so I'm'a just comment it out for now
                // todo: figure out what this is supposed to do n fix it
                // schemaProperty.SetCallbackUrl();
            }

        }

        private static void applyPropertyMetadata(OpenApiSchems schemaProperty, PropertyInfo propertyInfo)
        {

            // Apply friendly names and descriptions wherever possible
            // "x-ms-summary" - friendly name (applies to properties)
            // schema.properties["prop"].description - description (applies to parameters)

            if (schemaProperty == null || propertyInfo == null) return;

            var propertyMetadata = propertyInfo.GetCustomAttribute<MetadataAttribute>();

            if (propertyMetadata != null)
            {
                schemaProperty.SetVisibility(propertyMetadata.Visibility);
                schemaProperty.SetFriendlyNameAndDescription(propertyMetadata.FriendlyName, propertyMetadata.Description);
            }

        }
    }


}
