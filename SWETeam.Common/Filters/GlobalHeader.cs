using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.Filters
{
    /// <summary>
    /// Global add header filter
    /// </summary>
    public class GlobalHeader : IResultFilter
    {
        private readonly IServiceProvider _provider;
        public GlobalHeader(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers.Add("author", _provider.GetRequiredService<IConfiguration>().GetSection("Organization").Value);
        }
    }
}
