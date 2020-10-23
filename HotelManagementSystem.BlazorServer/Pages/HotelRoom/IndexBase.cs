using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Core;
using Business.DataModels;
using HotelManagementSystem.BlazorServer.Services;
using HotelManagementSystem.BlazorServer.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HotelManagementSystem.BlazorServer.Pages.HotelRoom
{
    public class IndexBase : ComponentBase
    {
        internal IEnumerable<HotelRoomDTO> HotelRooms { get; set; } = new List<HotelRoomDTO>();
        internal bool IsLoadComplete { get; set; } = false;
        internal int? DeleteRoomId { get; set; } = null;
        internal string SuccessMessage { get; set; }
        internal string ErrorMessage { get; set; }
        protected bool ShowConfirmation { get; set; } = false;

        [Inject]
        internal IHotelRepository HotelRepository { get; set; }
        [Inject]
        internal IHotelImagesRepository HotelImagesRepository { get; set; }
        [Inject]
        internal IJSRuntime JsRuntime { get; set; }
        [Inject]
        internal IAuthenticationService AuthService { get; set; }
        [Inject]
        internal NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (!await AuthService.IsUserAuthorized())
            {
                NavigationManager.NavigateTo("login",true);
                return;
            }
            ErrorMessage = "";
            SuccessMessage = "";
            IsLoadComplete = false;
            HotelRooms = await HotelRepository.GetAllHotelRooms();

            IsLoadComplete = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JsRuntime.InvokeAsync<object>("DataTable");
        }

        public void HandleDelete(int roomId)
        {
            DeleteRoomId = roomId;
            ShowConfirmation = true;
        }

        public async Task ConfirmDelete_Click(bool isConfirmed)
        {
            ErrorMessage = "";
            SuccessMessage = "";
            if (isConfirmed && DeleteRoomId != null)
            {
                try
                {
                    var result = await HotelRepository.DeleteHotelRoom(DeleteRoomId.Value,AuthService.User.Id);
                    SuccessMessage = "Hotel Room Deleted successfully";
                    ShowConfirmation = false;
                    await JsRuntime.InvokeVoidAsync("ShowToaster", "success", "Success", SuccessMessage);
                    NavigationManager.NavigateTo("/", true);
                }
                catch (Exception e)
                {
                    ErrorMessage = e.Message;
                    await JsRuntime.InvokeVoidAsync("ShowToaster", "error", "Error Occured", ErrorMessage);
                }
                HotelRooms = await HotelRepository.GetAllHotelRooms();
            }

            ShowConfirmation = false;
        }
    }
}
