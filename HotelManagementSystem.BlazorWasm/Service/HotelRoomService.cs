using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Business.DataModels;
using Business.Models;
using DataAccess.Data;
using HotelManagementSystem.BlazorWasm.Core;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Exception = System.Exception;

namespace HotelManagementSystem.BlazorWasm.Service
{
    public class HotelRoomService: IHotelRoomService
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorageService;
        private readonly NavigationManager _navigationManager;

        public HotelRoomService(HttpClient client, ILocalStorageService localStorageService, NavigationManager navigationManager)
        {
            _client = client;
            _localStorageService = localStorageService;
            _navigationManager = navigationManager;
        }

        public async Task<IEnumerable<HotelRoomDTO>> GetHotelRooms()
        {
            //var userDetails = await _localStorageService.GetItemAsync<UserDTO>("UserDetails");
            //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDetails.Token);
            var response = await _client.GetAsync("hotelrooms");

            if (await IsNotAuthorize(response)) return null;

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var rooms = JsonConvert.DeserializeObject<IEnumerable<HotelRoomDTO>>(content);
                return rooms;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorModel>(content);
                throw new Exception(error.ErrorMessage);
            }
            
        }

        public async Task<HotelRoomDTO> GetHotelRoomDetails(int roomId)
        {
            var userDetails = await _localStorageService.GetItemAsync<UserDTO>("UserDetails");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDetails.Token);
            var response = await _client.GetAsync($"hotelrooms/{roomId}");
            if (await IsNotAuthorize(response)) return null;
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var room = JsonConvert.DeserializeObject<HotelRoomDTO>(content);
                return room;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorModel>(content);
                throw new Exception(error.ErrorMessage);
            }
        }

        public async Task<SuccessModel> MarkAsBooked(int roomId)
        {
            var userDetails = await _localStorageService.GetItemAsync<UserDTO>("UserDetails");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDetails.Token);
            var roomDetails = await GetHotelRoomDetails(roomId);
            if (roomDetails != null)
            {
                var response = await _client.PostAsJsonAsync("hotelrooms/mark_as_booked", roomDetails);
                if (await IsNotAuthorize(response)) return null;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var room = JsonConvert.DeserializeObject<SuccessModel>(content);
                    return room;
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<ErrorModel>(content);
                    throw new Exception(error.ErrorMessage);
                }
            }
            else
            {
                throw new Exception("Room details not found");
            }
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


        public async Task<RoomOrderDetails> SaveRoomOrderDetails(RoomOrderDetails details)
        {
            var userDetails = await _localStorageService.GetItemAsync<UserDTO>("UserDetails");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDetails.Token);
            var response = await _client.PostAsJsonAsync("roomorder", details);
            if (await IsNotAuthorize(response)) return null;
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var room = JsonConvert.DeserializeObject<RoomOrderDetails>(content);
                return room;
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorModel>(content);
                throw new Exception(error.ErrorMessage);
            }
        }

        public async Task<RoomOrderDetails> MarkPaymentSuccessful(RoomOrderDetails details)
        {
            var userDetails = await _localStorageService.GetItemAsync<UserDTO>("UserDetails");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDetails.Token);
            var response = await _client.PostAsJsonAsync("RoomOrder/mark_payment_successful", details);
            if (await IsNotAuthorize(response)) return null;
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var room = JsonConvert.DeserializeObject<RoomOrderDetails>(content);
                return room;
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
