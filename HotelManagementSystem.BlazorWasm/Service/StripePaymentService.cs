using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Business.DataModels;
using Business.Models;
using HotelManagementSystem.BlazorWasm.Core;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace HotelManagementSystem.BlazorWasm.Service
{
    public class StripePaymentService: IStripePaymentService
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorageService;
        private readonly NavigationManager _navigationManager;

        public StripePaymentService(HttpClient client, ILocalStorageService localStorageService, NavigationManager navigationManager)
        {
            _client = client;
            _localStorageService = localStorageService;
            _navigationManager = navigationManager;
        }

        private async Task<bool> IsNotAuthorize(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _localStorageService.RemoveItemAsync("IsLoggedIn");
                await _localStorageService.RemoveItemAsync("UserDetails");
                _navigationManager.NavigateTo("login");
                return true;
            }

            return false;
        }

        public async Task<SuccessModel> Checkout(StripePaymentDTO model)
        {
            var userDetails = await _localStorageService.GetItemAsync<UserDTO>("UserDetails");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDetails.Token);
            var response = await _client.PostAsJsonAsync("StripePayment", model);

            if (await IsNotAuthorize(response)) return null;
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
    }
}
