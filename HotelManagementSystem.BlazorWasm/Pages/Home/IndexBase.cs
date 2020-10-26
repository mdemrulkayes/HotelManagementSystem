using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using HotelManagementSystem.BlazorWasm.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HotelManagementSystem.BlazorWasm.Pages.Home
{
    public class IndexBase: ComponentBase
    {
        public HomeModelVm HomeModel { get; set; } = new HomeModelVm();
        public bool IsProcessingStart { get; set; } = false;

        [Inject] public ILocalStorageService LocalStorage { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }


        public async Task SaveInitialData()
        {
            IsProcessingStart = true;
            try
            {
                if (HomeModel.EndDate < HomeModel.StartDate)
                {
                    await JsRuntime.InvokeVoidAsync("ShowToaster", "error", "Error Occured", "End Date must be greater than Start Date");
                    return;
                }

                var days = HomeModel.EndDate.Date.Subtract(HomeModel.StartDate.Date).TotalDays;
                HomeModel.TotalDay = Math.Abs(days) <= 0 ? 1 : days;
                await LocalStorage.SetItemAsync("InitialRoomBookingInfo", HomeModel);
                NavigationManager.NavigateTo("hotel/rooms", true);
            }
            catch (Exception e)
            {
                await JsRuntime.InvokeVoidAsync("ShowToaster", "error", "Error Occured", e.Message);
            }
            IsProcessingStart = false;
        }
    }
}
