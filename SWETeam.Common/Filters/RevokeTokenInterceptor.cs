using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using SWETeam.Common.Caching;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SWETeam.Common.Filters
{
    /// <summary>
    /// Filter kiểm tra xem token dã revoke hay chưa?
    /// </summary>
    public class RevokeTokenInterceptor : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var bearerToken = context.HttpContext.Request.Headers[HeaderNames.Authorization];
            if (!string.IsNullOrEmpty(bearerToken.ToString()))
            {
                var revokedToken = MemoryCacheHelper.Get(bearerToken.ToString());
                if (revokedToken != null)
                {
                    context.Result = new ContentResult()
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        ContentType = "application/json"
                    };
                    return;
                }
            }

        }
    }
}
