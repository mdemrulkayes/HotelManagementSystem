using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Serilog;

namespace HotelManagementSystem.Api.Middleware
{
    public class ErrorLoggerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                Log.Error(e, context.Request.GetDisplayUrl().ToString());
                await HandleExceptionAsync(context, e);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorList = new List<ErrorModel>()
            {
                new ErrorModel()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ErrorMessage = ex.ToString()
                }
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorList).ToString());
        }
    }
}
