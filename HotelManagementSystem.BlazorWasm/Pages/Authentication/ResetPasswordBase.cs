using System;
using System.Threading.Tasks;
using System.Web;
using Business.DataModels;
using HotelManagementSystem.BlazorWasm.Core;
using HotelManagementSystem.BlazorWasm.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace HotelManagementSystem.BlazorWasm.Pages.Authentication
{
    public class ResetPasswordBase: ComponentBase
    {

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public PasswordResetDTO ResetPasswordDto = new PasswordResetDTO(); 
        public ResetPasswordVm ResetPasswordVm = new ResetPasswordVm();
        public bool IsProcessStart { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        protected override void OnInitialized()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("userid",out var userId))
            {
                ResetPasswordVm.UserId = userId;
                
            }

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("token", out var token))
            {
                ResetPasswordVm.Token = HttpUtility.UrlEncode(token);
            }

            if (string.IsNullOrWhiteSpace(ResetPasswordVm.UserId) || string.IsNullOrWhiteSpace(ResetPasswordVm.Token))
            {
                NavigationManager.NavigateTo("/forgot-password");
            }
        }

        public async Task HandelResetPassword()
        {
            IsProcessStart = true;
            ErrorMessage = "";
            SuccessMessage = "";
            try
            {
                ResetPasswordDto = new PasswordResetDTO()
                {
                    UserId = ResetPasswordVm.UserId,
                    Token = HttpUtility.UrlDecode(ResetPasswordVm.Token),
                    NewPassword = ResetPasswordVm.NewPassword,
                    ConfirmNewPassword = ResetPasswordVm.ConfirmNewPassword
                };
                var result = await AuthenticationService.ResetPassword(ResetPasswordDto);
                SuccessMessage = result.SuccessMessage;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            IsProcessStart = false;
            ResetPasswordDto = new PasswordResetDTO();
            ResetPasswordVm = new ResetPasswordVm();
        }
    }
}
