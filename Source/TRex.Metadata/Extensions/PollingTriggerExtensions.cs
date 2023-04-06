/* 
 * PollingTriggerExtensions.cs
 * 
 *   Created: Sometime in 2016
 *   Modified: 2023-04-03-09:12:02
 * 
 *   Author: Nick Hauensteiin <Nicholas.Hauenstein@microsoft.com>
 *   Contributors: David G. Moore, Jr. david@dgmjr.io
 *                 CodeGPT (no really, it wrote a lot of this)
 *   
 *   Copyright © 2016 - 2023 Nick Hauenstein & David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */ 

using System;
using System.Buffers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TRex.Metadata.Extensions;

namespace TRex.Metadata
{
    public static class PollingTriggerExtensions
    {

        /// <summary>
        /// Generate a response message indicating that the polling trigger has data to return
        /// </summary>
        /// <typeparam name="TConfig">Type of the configuration data to pass to the polling operation (usually an anonymous type)</typeparam>
        /// <typeparam name="TResult">Type of the result that the polling trigger is returning</typeparam>
        /// <param name="request">Request for which to generate a response</param>
        /// <param name="result">Result that the polling trigger is returning</param>
        /// <param name="pollAgain">Timespan that indicates when the next polling operation should occur. This should be adjusted to a very small value in the case there is additional data available.</param>
        /// <param name="triggerRouteName">Name of the route that points to the polling operation that should be invoked at the next poll interval (typically specified in the Route attribute)</param>
        /// <param name="config">Object containing as members the parameters to pass to the polling operation. The names of the properties must exactly match the input parameters.</param>
        /// <returns>Returns an HttpResponseMessage that contains a result that can be used to trigger a Logic App or Flow</returns>
        public static HttpResponseMessage EventTriggered<TConfig, TResult>(this HttpRequest request,
            TResult result, TimeSpan pollAgain, string triggerRouteName, TConfig config)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            response.Content = new StringContent(JsonConvert.SerializeObject(result), System.Text.Encoding.UTF8, "application/json");
            response.Headers.RetryAfter = new RetryConditionHeaderValue(pollAgain);
            response.Headers.Location = new Uri(new UrlHelper(request.ToActionContext()).Link(triggerRouteName, config));

            return response;
        }

        /// <summary>
        /// Generate a response message indicating that the polling trigger has data to return
        /// </summary>
        /// <typeparam name="TConfig">Type of the configuration data to pass to the polling operation (usually an anonymous type)</typeparam>
        /// <typeparam name="TResult">Type of the result that the polling trigger is returning</typeparam>
        /// <param name="request">Request for which to generate a response</param>
        /// <param name="result">Result that the polling trigger is returning</param>
        /// <param name="triggerRouteName">Name of the route that points to the polling operation that should be invoked at the next poll interval (typically specified in the Route attribute)</param>
        /// <param name="config">Object containing as members the parameters to pass to the polling operation. The names of the properties must exactly match the input parameters.</param>
        /// <returns>Returns an HttpResponseMessage that contains a result that can be used to trigger a Logic App or Flow</returns>
        public static HttpResponseMessage EventTriggered<TConfig, TResult>(this HttpRequest request,
            TResult result, string triggerRouteName, TConfig config)
        {
            // Get an instance of IOptions<MvcNewtonsoftJsonOptions>
            var mvcJsonOptions = request.HttpContext.RequestServices.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>();
            var mvcOptions = request.HttpContext.RequestServices.GetRequiredService<IOptions<MvcOptions>>();

            // Create a NewtonsoftJsonOutputFormatter using the Newtonsoft.Json.JsonSerializer
            var formatter = new NewtonsoftJsonOutputFormatter(mvcJsonOptions.Value.SerializerSettings, ArrayPool<char>.Shared, mvcOptions.Value, mvcJsonOptions?.Value);

            var newtonsoftJsonMediaTypeFormatter = new NewtonsoftJsonMediaTypeFormatter();

            var response = new HttpResponseMessage(HttpStatusCode.OK);

            response.Content = new ObjectContent<TResult>(result, formatter.AsMediaTypeFormatter());
            response.Headers.Location = new Uri(new UrlHelper(request.ToActionContext()).Link(triggerRouteName, config));

            return response;
        }


        /// <summary>
        /// Generate a response message indicating that the polling trigger does not have data to return
        /// </summary>
        /// <typeparam name="T">Type of the configuration data to pass to the polling operation (usually an anonymous type)</typeparam>
        /// <param name="request">Request for which to generate a response</param>
        /// <param name="retryDelay">Timespan that indicates when the next polling operation should occur. Use TimeSpan.Zero to use the pre-configured defaults. This should be adjusted to a very small value in the case data is expected soon.</param>
        /// <param name="triggerRouteName">Name of the route that points to the polling operation that should be invoked at the next poll interval (typically specified in the Route attribute)</param>
        /// <param name="config">Object containing as members the parameters to pass to the polling operation. The names of the properties must exactly match the input parameters.</param>
        /// <returns>Returns an HttpResponseMessage that contains a result that can be used to avoid triggering a new instance of a Logic App or Flow</returns>
        public static HttpResponseMessage EventWaitPoll<T>(this HttpRequest request, TimeSpan retryDelay, string triggerRouteName, T config)
            where T : class
        {
            var response = new HttpResponseMessage(HttpStatusCode.Accepted);

            response.Headers.RetryAfter = new RetryConditionHeaderValue(retryDelay);
            response.Headers.Location = new Uri(new UrlHelper(request.ToActionContext()).Link(triggerRouteName, config));

            return response;
        }
    }

}
