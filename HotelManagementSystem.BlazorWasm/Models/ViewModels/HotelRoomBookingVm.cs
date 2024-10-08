﻿using Business.DataModels;
using DataAccess.Data;

namespace HotelManagementSystem.BlazorWasm.Models.ViewModels
{
    public class HotelRoomBookingVm
    {
        public HotelRoomDTO HotelRoom { get; set; }
        public StripePaymentDTO StripePaymentDto { get; set; }
        public RoomOrderDetails RoomOrderDetails { get; set; }
        public string ImageUrl { get; set; }
        
    }
}
