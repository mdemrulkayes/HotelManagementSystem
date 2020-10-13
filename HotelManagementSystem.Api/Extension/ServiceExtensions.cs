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
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
  
            return services;
        }
    }
}
