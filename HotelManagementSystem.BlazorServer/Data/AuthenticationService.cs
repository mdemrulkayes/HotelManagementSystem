using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Business.DataModels;
using Business.Models;
using HotelManagementSystem.BlazorServer.Services;
using Microsoft.AspNetCore.ProtectedBrowserStorage;
using Newtonsoft.Json;

namespace HotelManagementSystem.BlazorServer.Data
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _client;
        private readonly ProtectedSessionStorage _sessionStorage;

        public async Task Logout()
        {
            User = null;
            IsLoggedIn = false;
            await _sessionStorage.DeleteAsync("IsLoggedIn");
            await _sessionStorage.DeleteAsync("UserDetails");
        }

        public UserDTO User { get; private set; }
        public bool IsLoggedIn { get; private set; }

        public AuthenticationService(HttpClient client, ProtectedSessionStorage sessionStorage)
        {
            _client = client;
            _sessionStorage = sessionStorage;
        }

        public async Task<SuccessModel> SignUp(UserRequestDTO model)
        {
            var response = await _client.PostAsJsonAsync("account/signup", model);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SuccessModel>(content);
                return result;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorModel>(content);
                throw new Exception(error.ErrorMessage);
            }
        }

        public async Task<UserDTO> SignIn(AuthenticationDTO model)
        {
            var response = await _client.PostAsJsonAsync("account/signin", model);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<UserDTO>(content);
                await _sessionStorage.SetAsync("UserDetails", result);
                await _sessionStorage.SetAsync("IsLoggedIn", true);

                await Initialize();
                return result;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorModel>(content);
                throw new Exception(error.ErrorMessage);
            }
        }

        public async Task<SuccessModel> ForgotPassword(ForgotPasswordDTO model)
        {
            var response = await _client.PostAsJsonAsync("account/SendResetPasswordLink", model);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SuccessModel>(content);
                return result;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorModel>(content);
                throw new Exception(error.ErrorMessage);
            }
        }

        public async Task<SuccessModel> ResetPassword(PasswordResetDTO model)
        {
            var response = await _client.PostAsJsonAsync("account/ResetPassword", model);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SuccessModel>(content);
                return result;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorModel>(content);
                throw new Exception(error.ErrorMessage);
            }
        }

        public async Task<SuccessModel> ConfirmEmail(ConfirmEmailDTO model)
        {
            var response = await _client.PostAsJsonAsync("account/ConfirmEmail", model);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SuccessModel>(content);
                return result;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorModel>(content);
                throw new Exception(error.ErrorMessage);
            }
        }

        public async Task Initialize()
        {
            User = await _sessionStorage.GetAsync<UserDTO>("UserDetails");
            IsLoggedIn = await _sessionStorage.GetAsync<bool>("IsLoggedIn");
        }

        public async Task<bool> IsUserAuthorized()
        {
            if (User == null)
            {
                User = await _sessionStorage.GetAsync<UserDTO>("UserDetails");
                IsLoggedIn = await _sessionStorage.GetAsync<bool>("IsLoggedIn");
            }
            if (IsLoggedIn && User != null && (User.Role == "Admin" || User.Role == "Developer"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
