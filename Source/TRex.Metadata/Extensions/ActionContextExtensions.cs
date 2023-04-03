/* 
 * ActionContextExtensions.cs
 * 
 *   Created: 2023-04-03-02:53:00
 *   Modified: 2023-04-03-02:53:00
 * 
 *   Author: Justin Chase <justin@justinwritescode.com>
 *   
 *   Copyright © 2022-2023 Justin Chase, All Rights Reserved
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
