using System;
using System.Net;
using Cardofun.API.Helpers.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

        /// <summary>
        /// Adds "Pagination" header to the response that contains
        /// information about the current returned page, total of pages, etc
        /// </summary>
        /// <param name="response"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="totalItems"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public static HttpResponse AddPagination(this HttpResponse response, 
            Int32 currentPage, Int32 itemsPerPage, Int32 totalItems, Int32 totalPages)
        {
            var pagination = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

            response.Headers.Add("Pagination", 
                JsonConvert.SerializeObject(pagination, GetCamelCaseFormatter()));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");

            return response;
        }

        private static JsonSerializerSettings GetCamelCaseFormatter()
        {
            var formatter = new JsonSerializerSettings();
            formatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return formatter;
        }
    }
}