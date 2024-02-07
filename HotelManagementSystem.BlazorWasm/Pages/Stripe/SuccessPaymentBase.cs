using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using DataAccess.Data;
using HotelManagementSystem.BlazorWasm.Core;
using Microsoft.AspNetCore.Components;

namespace HotelManagementSystem.BlazorWasm.Pages.Stripe
{
    public class SuccessPaymentBase: ComponentBase
    {
        public bool IsPaymentComplete { get; set; } = false;
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public int OrderId { get; set; }
        [Inject]
        public ILocalStorageService LocalStorageService { get; set; }
        [Inject]
        public IHotelRoomService HotelRoomService { get; set; }


        protected override async Task OnInitializedAsync()
        {
            ErrorMessage = "";
            SuccessMessage = "";
            var orderDetails = await LocalStorageService.GetItemAsync<RoomOrderDetails>("OrderDetails");
            var roomId = await LocalStorageService.GetItemAsync<int>("RoomId");
            OrderId = orderDetails.Id;
            try
            {
               var paymentResult = await HotelRoomService.MarkPaymentSuccessful(orderDetails);
               var roomBookResult = await HotelRoomService.MarkAsBooked(roomId);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }

            await LocalStorageService.RemoveItemAsync("OrderDetails");
            await LocalStorageService.RemoveItemAsync("RoomId");
            await LocalStorageService.RemoveItemAsync("InitialRoomBookingInfo");

            IsPaymentComplete = true;
        }
    }
}
