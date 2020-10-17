using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.DataModels;
using HotelManagementSystem.BlazorWasm.Core;
using Microsoft.AspNetCore.Components;

namespace HotelManagementSystem.BlazorWasm.Pages.Authentication
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
