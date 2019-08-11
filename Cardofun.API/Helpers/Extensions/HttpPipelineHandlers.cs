using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Cardofun.API.Helpers.Extensions
{
    /// <summary>
    /// Class that exposes extensions for building HTTP pipeline
    /// </summary>
    public static class HttpPipelineHandlers
    {
        /// <summary>
        /// Adds CORS headers exposing Application-Error header,
        /// that contains the error thrown by any exception throught the app
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder AddGlobalErrorHandling(this IApplicationBuilder builder)
        {
            builder.Run(async context => 
            {
                var error = context.Features.Get<IExceptionHandlerFeature>();
                if(error == null)
                    return;
                
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
                context.Response.Headers.Add("Application-Error", error.Error.Message);

                await context.Response.WriteAsync(error.Error.Message);
            });

            return builder;
        }
    }
}