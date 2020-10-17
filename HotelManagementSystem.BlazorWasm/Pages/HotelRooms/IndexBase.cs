using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Business.DataModels;
using Business.Models;
using HotelManagementSystem.BlazorWasm.Core;
using Microsoft.AspNetCore.Components;

namespace HotelManagementSystem.BlazorWasm.Pages.HotelRooms
{
    public class IndexBase: ComponentBase
    {
        [Inject] public IHotelRoomService HotelRoomService { get; set; }
        [Parameter]
        public IEnumerable<HotelRoomDTO> Rooms { get; set; } = new List<HotelRoomDTO>();
        [Parameter]
        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Rooms = await HotelRoomService.GetHotelRooms();
                
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }

        }
    }
}
