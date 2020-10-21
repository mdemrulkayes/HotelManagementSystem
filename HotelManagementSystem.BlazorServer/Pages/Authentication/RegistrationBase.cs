﻿using Business.DataModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotelManagementSystem.BlazorServer.Models.ViewModels;
using HotelManagementSystem.BlazorServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace HotelManagementSystem.BlazorServer.Pages.Authentication
{
    public class RegistrationBase : ComponentBase
    {
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }

        public UserRequestDTO userRequestDTO = new UserRequestDTO();
        public UserRegistrationVm UserRegistrationVm = new UserRegistrationVm();
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public bool IsProcessStart { get; set; } = false;

        public async Task HandelRegistration()
        {
            IsProcessStart = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            try
            {
                userRequestDTO = new UserRequestDTO()
                {
                    Name = UserRegistrationVm.Name,
                    Email = UserRegistrationVm.Email,
                    PhoneNo = UserRegistrationVm.PhoneNo,
                    Password = UserRegistrationVm.Password,
                    ConfirmPassword = UserRegistrationVm.ConfirmPassword,
                    UserRole = UserRegistrationVm.UserRole
                };
                var result = await AuthenticationService.SignUp(userRequestDTO);
                if (result.StatusCode == StatusCodes.Status200OK)
                {
                    SuccessMessage = result.SuccessMessage;
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }

            IsProcessStart = false;
            UserRegistrationVm = new UserRegistrationVm();

        }
    }
}