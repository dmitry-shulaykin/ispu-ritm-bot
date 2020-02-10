using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradesNotification.Middlewares
{
    public static class LoggerMiddleWareExtensions
    {
        public static IApplicationBuilder UseErrorLoggingMiddleware(
              this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggerMiddleware>();
        }
    }
}
