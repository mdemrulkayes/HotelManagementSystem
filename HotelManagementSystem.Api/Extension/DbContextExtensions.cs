using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagementSystem.Api.Extension
{
    public static class DbContextExtensions
    {
        public static IServiceCollection DbService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CoreDbContext>(
                opt => { opt.UseSqlServer(configuration.GetConnectionString("DBCS")); }, ServiceLifetime.Scoped);

            return services;
        }
    }
}
