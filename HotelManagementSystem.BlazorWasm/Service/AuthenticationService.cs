using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Business.DataModels;
using Business.Models;
using HotelManagementSystem.BlazorWasm.Core;
using HotelManagementSystem.BlazorWasm.Models.ViewModels;
using Newtonsoft.Json;

namespace HotelManagementSystem.BlazorWasm.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorageService;

        public async Task Logout()
        {
            User = null;
            IsLoggedIn = false;
            await _localStorageService.RemoveItemAsync("IsLoggedIn");
            await _localStorageService.RemoveItemAsync("UserDetails");
        }

        public UserDTO User { get; private set; }
        public bool IsLoggedIn { get; private set; }

        public AuthenticationService(HttpClient client, ILocalStorageService localStorageService)
        {
            _client = client;
            _localStorageService = localStorageService;
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
                await _localStorageService.SetItemAsync("UserDetails", result);
                await _localStorageService.SetItemAsync("IsLoggedIn", true);
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

        public async Task<UserDTO> SigninInWithFacebook(FbResponseVm model)
        {
            var apiModel = new FacebookAuthenticationDto()
            {
                Name = model.first_name + " " +model.last_name,
                Email = model.email,
                FbId = model.id
            };
            var response = await _client.PostAsJsonAsync("account/SigninWithFacebook", apiModel);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<UserDTO>(content);
                await _localStorageService.SetItemAsync("UserDetails", result);
                await _localStorageService.SetItemAsync("IsLoggedIn", true);
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
            User = await _localStorageService.GetItemAsync<UserDTO>("UserDetails");
            IsLoggedIn = await _localStorageService.GetItemAsync<bool>("IsLoggedIn");
        }
    }
}
