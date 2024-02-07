using System;
using System.Threading.Tasks;
using Business.DataModels;
using HotelManagementSystem.BlazorServer.Services;
using Microsoft.AspNetCore.Components;

namespace HotelManagementSystem.BlazorServer.Pages.Authentication
{
    public class ForgotPasswordBase: ComponentBase
    {
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        public ForgotPasswordDTO ForgotPassword { get; set; } = new ForgotPasswordDTO();
        
        public bool IsProcessStart { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public async Task HandelForgotPassword()
        {
            IsProcessStart = true;
            ErrorMessage = "";
            SuccessMessage = "";
            try
            {
                var result = await AuthenticationService.ForgotPassword(ForgotPassword);
                SuccessMessage = result.SuccessMessage;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            ForgotPassword = new ForgotPasswordDTO();
            IsProcessStart = false;
        }
    }
}
