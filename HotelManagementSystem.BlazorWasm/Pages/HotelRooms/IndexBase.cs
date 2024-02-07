using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Business.DataModels;
using HotelManagementSystem.BlazorWasm.Core;
using HotelManagementSystem.BlazorWasm.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HotelManagementSystem.BlazorWasm.Pages.HotelRooms
{
    public class IndexBase: ComponentBase
    {
        internal HomeModelVm HomeModel { get; set; } = new HomeModelVm();
        internal bool IsProcessingStart { get; set; } = false;
        public IEnumerable<HotelRoomDTO> Rooms { get; set; } = new List<HotelRoomDTO>();
        public string ErrorMessage { get; set; }

        [Inject] public IHotelRoomService HotelRoomService { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public ILocalStorageService LocalStorage { get; set; }


        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (await LocalStorage.GetItemAsync<HomeModelVm>("InitialRoomBookingInfo") != null)
                {
                    HomeModel = await LocalStorage.GetItemAsync<HomeModelVm>("InitialRoomBookingInfo");
                }
                else
                {
                    HomeModel.TotalDay = 1;
                }

                await LoadRooms();
            }
            catch (Exception e)
            {
                await JsRuntime.InvokeVoidAsync("ShowToaster", "error", "Error Occured", "End Date must be greater than Start Date");
            }

        }

        private async Task LoadRooms()
        {
            Rooms = await HotelRoomService.GetHotelRooms();
            foreach (var room in Rooms)
            {
                room.TotalDays = HomeModel.TotalDay;
                room.TotalAmount = room.RegularRate * Convert.ToDecimal(HomeModel.TotalDay);
            }
        }

        public async Task SaveBookingInfo()
        {
            IsProcessingStart = true;
            if (HomeModel.EndDate < HomeModel.StartDate)
            {
                await JsRuntime.InvokeVoidAsync("ShowToaster", "error", "Error Occured", "End Date must be greater than Start Date");
                return;
            }
            HomeModel.TotalDay = HomeModel.EndDate.Date.Subtract(HomeModel.StartDate.Date).Days;
            await LocalStorage.SetItemAsync("InitialRoomBookingInfo", HomeModel);
            await LoadRooms();
            IsProcessingStart = false;
        }
    }
}
