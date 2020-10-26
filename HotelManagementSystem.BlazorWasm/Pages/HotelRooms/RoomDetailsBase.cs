using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Business.DataModels;
using DataAccess.Data;
using HotelManagementSystem.BlazorWasm.Core;
using HotelManagementSystem.BlazorWasm.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HotelManagementSystem.BlazorWasm.Pages.HotelRooms
{
    public class RoomDetailsBase : ComponentBase
    {
        [Parameter]
        public int? Id { get; set; }
        
        public HotelRoomBookingVm HotelRoomBooking { get; set; } = new HotelRoomBookingVm();
        public string ErrorMessage { get; set; }
        public bool IsProcessStart { get; set; }
        [Inject]
        public IHotelRoomService HotelRoomService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public IStripePaymentService StripePaymentService { get; set; }
        [Inject]
        public ILocalStorageService LocalStorageService { get; set; }


        protected override async Task OnInitializedAsync()
        {
            if (Id == null)
            {
                NavigationManager.NavigateTo("hotel/rooms");
            }

            try
            {
               
                HotelRoomBooking.RoomOrderDetails = new RoomOrderDetails();
                if (Id != null)
                {
                    HotelRoomBooking.HotelRoom = await HotelRoomService.GetHotelRoomDetails(Id.Value);
                    HotelRoomBooking.ImageUrl = HotelRoomBooking.HotelRoom.HotelRoomImages.Count > 0 ? HotelRoomBooking.HotelRoom.HotelRoomImages.FirstOrDefault()?.RoomImageUrl : "";

                    if (await LocalStorageService.GetItemAsync<HomeModelVm>("InitialRoomBookingInfo") != null)
                    {
                        var roomInitialInfo = await LocalStorageService.GetItemAsync<HomeModelVm>("InitialRoomBookingInfo");
                        HotelRoomBooking.RoomOrderDetails.CheckInDate = roomInitialInfo.StartDate;
                        HotelRoomBooking.RoomOrderDetails.CheckOutDate = roomInitialInfo.EndDate;
                        HotelRoomBooking.HotelRoom.TotalDays = roomInitialInfo.TotalDay;
                        HotelRoomBooking.HotelRoom.TotalAmount = Convert.ToDecimal(roomInitialInfo.TotalDay) * HotelRoomBooking.HotelRoom.RegularRate;
                    }
                    else
                    {
                        HotelRoomBooking.RoomOrderDetails.CheckInDate = DateTime.UtcNow;
                        HotelRoomBooking.RoomOrderDetails.CheckOutDate = DateTime.UtcNow.AddDays(1);
                        HotelRoomBooking.HotelRoom.TotalDays = 1;
                        HotelRoomBooking.HotelRoom.TotalAmount = HotelRoomBooking.HotelRoom.RegularRate;
                    }

                    if (await LocalStorageService.GetItemAsync<UserDTO>("UserDetails") != null)
                    {
                        var userInfo = await LocalStorageService.GetItemAsync<UserDTO>("UserDetails");
                        HotelRoomBooking.RoomOrderDetails.Name = userInfo.Name;
                        HotelRoomBooking.RoomOrderDetails.Email = userInfo.Email;
                    }
                }
                
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }

        }

        //protected override async Task OnAfterRenderAsync(bool h)
        //{
        //    if (firstRender)
        //    {
        //        await JsRuntime.InvokeAsync<object>("initializeCarousel");
        //        firstRender = false;
        //    }
        //}

        public async Task HandleCheckout()
        {
            IsProcessStart = true;
            var userDetailsOnSignIn = await LocalStorageService.GetItemAsync<UserDTO>("UserDetails");
            try
            {
                var totalDays = HotelRoomBooking.RoomOrderDetails.CheckOutDate.Date
                    .Subtract(HotelRoomBooking.RoomOrderDetails.CheckInDate.Date).Days;
                
                if (totalDays <= 0)
                {
                    totalDays = 1;
                }
                var paymentDto = new StripePaymentDTO()
                {
                    //UserId = "e17daf5e-be34-447d-ab1b-1202b438dc49",
                    Amount = Convert.ToInt64(Convert.ToDecimal(totalDays) * HotelRoomBooking.HotelRoom.RegularRate),
                    ProductName = HotelRoomBooking.HotelRoom.Name,
                    ImageUrl = HotelRoomBooking.ImageUrl
                };
                

                var result = await StripePaymentService.Checkout(paymentDto);

                #region Store Order details without payment successful status and room is not booked yet

                HotelRoomBooking.RoomOrderDetails.StripeSessionId = result.Data.ToString();
                HotelRoomBooking.RoomOrderDetails.RoomId = HotelRoomBooking.HotelRoom.Id;
                HotelRoomBooking.RoomOrderDetails.TotalCost =
                    Convert.ToInt64(Convert.ToDecimal(totalDays) * HotelRoomBooking.HotelRoom.RegularRate);
                HotelRoomBooking.RoomOrderDetails.UserId = userDetailsOnSignIn.Id;

                var roomOrderDetailsSavedResult = await HotelRoomService.SaveRoomOrderDetails(HotelRoomBooking.RoomOrderDetails);

                await LocalStorageService.SetItemAsync("OrderDetails", roomOrderDetailsSavedResult);
                await LocalStorageService.SetItemAsync("RoomId", HotelRoomBooking.HotelRoom.Id);
                
                #endregion

                await JsRuntime.InvokeVoidAsync("redirectToCheckout", result.Data.ToString());
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }

            IsProcessStart = false;
            HotelRoomBooking.HotelRoom = new HotelRoomDTO();
            HotelRoomBooking.RoomOrderDetails = new RoomOrderDetails();
        }

    }
}
