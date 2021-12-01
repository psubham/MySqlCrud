using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Mime;


namespace Capital.Exceptions
{
    public static class ExceptionMiddlewareExtension
    {
        /// <summary>
        /// Adds a middleware to your web application pipeline to handle exception globally.
        /// </summary>
        /// <param name="app">Application Builder.</param>
        /// <param name="logger">Logger.</param>
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app, ILogger<Startup> logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = MediaTypeNames.Application.Json;

                    IExceptionHandlerFeature contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError($"Something went wrong: {contextFeature.Error}");

                        await context.Response.WriteAsync(new ErrorDetail()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message
                        }.ToString());
                    }
                });
            });
        }
    }
}
