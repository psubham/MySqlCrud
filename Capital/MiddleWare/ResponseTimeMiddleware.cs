using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Capital.MiddleWare
{
    /// <summary>
    /// Adds a middleware to your web application pipeline to cslculate response time.
    /// </summary>
    public class ResponseTimeMiddleware
    {
        // Name of the Response Header, Custom Headers starts with "X-"  
        private const string RESPONSE_HEADER_RESPONSE_TIME = "X-Response-Time-ms";
        // Handle to the next Middleware in the pipeline  
        private readonly RequestDelegate _next;
        ///<summary>
        ///</summary>
        public ResponseTimeMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        ///<summary>
        ///</summary>
        public Task InvokeAsync(HttpContext context)
        {
            // Start the Timer using Stopwatch  
            var watch = new Stopwatch();
            watch.Start();
            context.Response.OnStarting(() => {
                // Stop the timer information and calculate the time   
                watch.Stop();
                var responseTimeForCompleteRequest = watch.ElapsedMilliseconds;
                // Add the Response time information in the Response headers.   
                context.Response.Headers[RESPONSE_HEADER_RESPONSE_TIME] = responseTimeForCompleteRequest.ToString();
                return Task.CompletedTask;
            });
            // Call the next delegate/middleware in the pipeline   
            return this._next(context);
        }
    }
    /// <summary>
    /// use middleware for runtime calculation
    /// </summary>
    public static class MyMiddlewareExtensions
    {
        public static IApplicationBuilder UseResponseTimeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseTimeMiddleware>();
        }
    }
}
