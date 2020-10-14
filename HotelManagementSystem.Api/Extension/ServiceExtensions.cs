using Business.Core;
using Business.Persistence;
using DataAccess.Data;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagementSystem.Api.Extension
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IHotelImagesRepository, HotelImagesRepository>();
  
            return services;
        }
    }
}
