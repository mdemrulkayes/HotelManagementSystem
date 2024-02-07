using System;
using System.Threading.Tasks;
using System.Web;
using Business.DataModels;
using HotelManagementSystem.BlazorWasm.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace HotelManagementSystem.BlazorWasm.Pages.Authentication
{
    public class ConfirmEmailBase : ComponentBase
    {
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public ConfirmEmailDTO ConfirmEmailDto = new ConfirmEmailDTO();
        public string UserId { get; set; }
        public string Code { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ErrorMessage = "";
            SuccessMessage = "";
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("userId", out var userId))
            {
                UserId = userId;
            }

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("code", out var code))
            {
                Code = HttpUtility.UrlEncode(code);
            }

            if (string.IsNullOrWhiteSpace(UserId) || string.IsNullOrWhiteSpace(Code))
            {
                NavigationManager.NavigateTo("/login");
                return;
            }

            try
            {
                ConfirmEmailDto.Code = HttpUtility.UrlDecode(Code);
                ConfirmEmailDto.UserId = UserId;
                var result = await AuthenticationService.ConfirmEmail(ConfirmEmailDto);
                SuccessMessage = result.SuccessMessage;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

    }
}
