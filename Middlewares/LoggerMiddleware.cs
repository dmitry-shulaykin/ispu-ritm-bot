using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using GradesNotification.Extensions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace GradesNotification
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<LoggerMiddleware> logger)
        {
            try
            {
                using var task = (logger).GetTelemetryEventDisposable($"[{context.Request.Method}] {context.Request.Path}");
                await _next(context);
            } 
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e, logger);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
        {
            logger.LogError(exception.ToString());
            object payload = null;

            string result = JsonConvert.SerializeObject(new { ok = false, error = exception.Message, payload });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync(result);
        }


    }
}
