﻿namespace QuickLearn.ApiApps.Metadata
{

    /// <summary>
    /// Contains all of the required magical constants
    /// </summary>
    internal static class Constants
    {

        #region Microsoft Vendor Extensions

        /// <summary>
        /// Gets the string "x-ms-summary"
        /// </summary>
        internal const string X_MS_SUMMARY = "x-ms-summary";
        
        /// <summary>
        /// Gets or sets the string "x-ms-notification-url"
        /// </summary>
        internal const string X_MS_NOTIFICATION_URL = "x-ms-notification-url";

        /// <summary>
        /// Gets the string "x-ms-visibility"
        /// </summary>
        internal const string X_MS_VISIBILITY = "x-ms-visibility";

        /// <summary>
        /// Gets the string "x-ms-notification-content"
        /// </summary>
        internal const string X_MS_NOTIFICATION_CONTENT = "x-ms-notification-content";

        /// <summary>
        /// Gets the string "x-ms-trigger";
        /// </summary>
        internal const string X_MS_TRIGGER = "x-ms-trigger";

        /// <summary>
        /// Gets the string "x-ms-dynamic-values";
        /// </summary>
        internal const string X_MS_DYNAMIC_VALUES = "x-ms-dynamic-values";

        /// <summary>
        /// Gets the string "x-ms-dynamic-schema";
        /// </summary>
        internal const string X_MS_DYNAMIC_SCHEMA = "x-ms-dynamic-schema";

        #endregion
        
        #region Response Descriptions

        /// <summary>
        /// Gets the string "default"
        /// </summary>
        internal const string DEFAULT_RESPONSE_KEY = "default";

        /// <summary>
        /// Gets the response code "200"
        /// </summary>
        internal const string HAPPY_POLL_WITH_DATA_RESPONSE_CODE = "200";

        /// <summary>
        /// Gets the response code "202"
        /// </summary>
        internal const string HAPPY_POLL_NO_DATA_RESPONSE_CODE = "202";

        /// <summary>
        /// Gets the first digit ("2") of the 200 level of response codes
        /// </summary>
        internal const string HAPPY_RESPONSE_CODE_LEVEL_START = "2";
        

        #endregion

    }
}
