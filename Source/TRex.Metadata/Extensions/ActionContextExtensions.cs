/* 
 * ActionContextExtensions.cs
 * 
 *   Created: Sometime in 2016
 *   Modified: 2023-04-03-09:11:40
 * 
 *   Author: Nick Hauensteiin <Nicholas.Hauenstein@microsoft.com>
 *   Contributors: David G. Moore, Jr. david@dgmjr.io
 *                 CodeGPT (no really, it wrote a lot of this)
 *   
 *   Copyright © 2016 - 2023 Nick Hauenstein & David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */ 


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace TRex.Metadata.Extensions;

public static class ActionContextExtensions
{

    public static ActionContext ToActionContext(this HttpRequest request)
    {
        // assuming you have an HttpRequest object named "request"
        HttpContext httpContext = request.HttpContext;
        Endpoint endpoint = httpContext.GetEndpoint();
        ActionContext actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), new ControllerActionDescriptor());
        return actionContext;

    }
}
