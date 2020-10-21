using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Business.DataModels;
using HotelManagementSystem.BlazorWasm.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace HotelManagementSystem.BlazorWasm.Helpers
{
    public class AppRouteView : RouteView
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }

        protected override void Render(RenderTreeBuilder builder)
        {
            var authorize = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute)) != null;
            var isLoggedIn = AuthenticationService.IsLoggedIn;
            var userDetails = AuthenticationService.User;


            if (authorize && isLoggedIn == false && userDetails == null)
            {
                var returnUrl = WebUtility.UrlEncode(new Uri(NavigationManager.Uri).PathAndQuery);
                NavigationManager.NavigateTo("login");
            }
            else
            {
                base.Render(builder);
            }
        }
    }
}
