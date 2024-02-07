using System.Threading.Tasks;
using Business.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.ProtectedBrowserStorage;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagementSystem.BlazorServer.Attributes
{
    public class CustomAuthorize: AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        //public string Roles { get; set; }
        

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var sessionStorage = context.HttpContext.RequestServices.GetRequiredService<ProtectedSessionStorage>();
            if (string.IsNullOrEmpty(Roles))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var isUserLoggedIn = await sessionStorage.GetAsync<bool>("IsLoggedIn");
            var userDetails = await sessionStorage.GetAsync<UserDTO>("UserDetails");

            if (isUserLoggedIn && userDetails != null)
            {
                return;
            }
            context.Result = new UnauthorizedResult();
            return;
        }
    }
}
