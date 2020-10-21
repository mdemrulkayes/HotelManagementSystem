﻿using Business.DataModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using DataAccess.Data;
using HotelManagementSystem.BlazorServer.Services;
using Microsoft.AspNetCore.Identity;

namespace HotelManagementSystem.BlazorServer.Pages.Authentication
{
    public class LoginBase : ComponentBase
    {
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public AuthenticationDTO authenticationDTO = new AuthenticationDTO();
        public bool IsProcessStart { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public async Task HandelLogin()
        {
            IsProcessStart = true;
            ErrorMessage = "";
            SuccessMessage = "";
            try
            {
                var authResult = await AuthenticationService.SignIn(authenticationDTO);
                
                if (authResult != null)
                {
                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    ErrorMessage = "Username and password does not match";
                }
                
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }

            IsProcessStart = false;
            authenticationDTO = new AuthenticationDTO();
        }
    }
}
