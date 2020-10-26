using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using HotelManagementSystem.BlazorWasm.Core;
using HotelManagementSystem.BlazorWasm.Service;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace HotelManagementSystem.BlazorWasm
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            
            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri("https://localhost:44388/api/") });
            //builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri("http://localhost:51110") });
            builder.Services.AddScoped<IHotelRoomService, HotelRoomService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IStripePaymentService, StripePaymentService>();
            builder.Services.AddBlazoredLocalStorage();

            var host = builder.Build();

            var authenticationService = host.Services.GetRequiredService<IAuthenticationService>();
            await authenticationService.Initialize();

            await host.RunAsync();
        }
    }
}
