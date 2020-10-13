using HotelManagementSystem.Api.Middleware;
using Microsoft.AspNetCore.Builder;

namespace HotelManagementSystem.Api.Extension
{
    public static class CustomErrorLoggingExtensions
    {
        public static void UserCustomErrorLogging(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ErrorLoggerMiddleware>();
        }
    }
}
