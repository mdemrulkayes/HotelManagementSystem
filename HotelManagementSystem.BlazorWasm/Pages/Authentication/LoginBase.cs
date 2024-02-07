using Business.DataModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using HotelManagementSystem.BlazorWasm.Core;
using HotelManagementSystem.BlazorWasm.Models.ViewModels;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace HotelManagementSystem.BlazorWasm.Pages.Authentication
{
    public class LoginBase : ComponentBase
    {
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        public AuthenticationDTO authenticationDTO = new AuthenticationDTO();
        public bool IsProcessStart { get; set; }
        public static string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public static bool IsFbLoginProcessStart { get; set; }
        public static FbResponseVm FbResponseData { get; set; } = new FbResponseVm();

        private static Action Action;
        protected override void OnInitialized()
        {
            Action = async () =>
            {
                await SigninWithFb();
            };
        }

        public async Task HandelLogin()
        {
            IsProcessStart = true;
            ErrorMessage = "";
            SuccessMessage = "";
            try
            {
                var authResult =await AuthenticationService.SignIn(authenticationDTO);

                var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("returnUrl", out var rerunUrl))
                {
                    NavigationManager.NavigateTo(rerunUrl.ToString(), true);
                }
                else
                {
                    NavigationManager.NavigateTo("/");
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }

            IsProcessStart = false;
            authenticationDTO = new AuthenticationDTO();
        }

        public async Task HandleFbLogin()
        {
            IsFbLoginProcessStart = true;
            var data = await JsRuntime.InvokeAsync<object>("fbLogin");
        }

        private async Task SigninWithFb()
        {
            try
            {
                var authResult = await AuthenticationService.SigninInWithFacebook(FbResponseData);

                var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("returnUrl", out var rerunUrl))
                {
                    NavigationManager.NavigateTo(rerunUrl.ToString(), true);
                }
                else
                {
                    NavigationManager.NavigateTo("/");
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            IsFbLoginProcessStart = false;
        }

        [JSInvokable("FbLoginProcessCallback")]
        public static void FbLoginProcessCallback(object result)
        {
            FbResponseData = JsonConvert.DeserializeObject<FbResponseVm>(result.ToString());
            Action.Invoke();
            IsFbLoginProcessStart = false;
        }
    }
}
