using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TRex.Metadata;
using TRex.Metadata.Models;


namespace QuickLearn.ApiApps.Metadata.Extensions
{
    internal static class OperationExtensions
    {
        public static void SetTrigger(this OpenApiOperation operation, ISchemaGenerator schemaRegistry, TriggerAttribute triggerDescription)
        {
            string batchMode = null;


            switch (triggerDescription.Pattern)
            {
                case TriggerType.PollingBatched:
                    batchMode = Constants.BATCH;
                    break;
                case TriggerType.PollingSingle:
                    batchMode = Constants.SINGLE;
                    break;
                case TriggerType.Subscription:
                    batchMode = Constants.SINGLE;
                    operation.SetCallbackType(schemaRegistry, triggerDescription.DataType, triggerDescription.DataFriendlyName);
                    break;
                default:
                    break;
            }

            if (!operation.CustomAttributes.Contains(new KeyValuePair<string, object>(Constants.X_MS_TRIGGER, batchMode.ToString().ToLowerInvariant())))
            {
                operation.CustomAttributes.Add(new KeyValuePair<string, object>(Constants.X_MS_TRIGGER,
                    batchMode.ToString().ToLowerInvariant()));
            }

            if (triggerDescription.Pattern == TriggerType.Subscription) return;

            var dataResponse = new OpenApiResponse()
            {
                Description = triggerDescription.DataFriendlyName,
                Content = new Dictionary<string, OpenApiMediaType>
            {
                { "application/json",
                    new OpenApiMediaType()
                    {
                        Schema = null != triggerDescription.DataType
                            ? schemaRegistry.GenerateSchema(triggerDescription.DataType, null)
                            : null
                    }
                }
            }
            };

            var acceptedResponse = new OpenApiResponse()
            {
                Description = Constants.ACCEPTED
            };

            operation.Responses.Add(Constants.HAPPY_POLL_WITH_DATA_RESPONSE_CODE, dataResponse);
            operation.Responses.Add(Constants.HAPPY_POLL_NO_DATA_RESPONSE_CODE, acceptedResponse);

        }

        public static void SetCallbackType(this OpenApiOperation operation, ISchemaGenerator schemaRegistry, System.Type callbackType, string description)
        {
            if (!callbackType.GetCustomAttributes(true).Contains(new KeyValuePair<string, object>(Constants.X_MS_NOTIFICATION_CONTENT, null)))
            {
                var schemaInfo = schemaRegistry.GenerateSchema(callbackType, null);

                var notificationData = new NotificationContentModel()
                {
                    Description = description,
                    Schema = new SchemaModel(schemaInfo)
                };

                operation.CustomAttributes.Add(new KeyValuePair<string, object>(Constants.X_MS_NOTIFICATION_CONTENT,
                    notificationData));
            }
        }

        public static void SetVisibility(this OpenApiOperation operation, VisibilityType visibility)
        {
            if (visibility == VisibilityType.Default) return;

            if (!operation.CustomAttributes.Contains(new KeyValuePair<string, object>(Constants.X_MS_VISIBILITY, visibility.ToString().ToLowerInvariant())))
                operation.CustomAttributes.Add(new KeyValuePair<string, object>(Constants.X_MS_VISIBILITY,
                    visibility));

        }

        public static void SetFriendlyNameAndDescription(this OpenApiOperation operation, string friendlyName, string description)
        {
            if (!string.IsNullOrWhiteSpace(description))
                operation.Summary = description;

            if (!string.IsNullOrWhiteSpace(friendlyName))
            {
                operation.Summary = friendlyName;
                operation.OperationId = GetOperationId(friendlyName);
            }
        }

        public static string GetOperationId(string friendlyName)
        {
            if (!friendlyName.Contains(" ")) return friendlyName;

            return CultureInfo.CurrentCulture.TextInfo
                .ToTitleCase(friendlyName)
                .Replace(" ", string.Empty);
        }
    }
}
