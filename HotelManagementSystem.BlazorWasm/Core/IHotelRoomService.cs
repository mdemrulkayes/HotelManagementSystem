using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.DataModels;
using Business.Models;
using DataAccess.Data;

namespace HotelManagementSystem.BlazorWasm.Core
{
    public interface IHotelRoomService
    {
        public Task<IEnumerable<HotelRoomDTO>> GetHotelRooms();
        public Task<HotelRoomDTO> GetHotelRoomDetails(int roomId);
        public Task<SuccessModel> MarkAsBooked(int roomId);
        public Task<RoomOrderDetails> SaveRoomOrderDetails(RoomOrderDetails details);
        public Task<RoomOrderDetails> MarkPaymentSuccessful(RoomOrderDetails details);
    }
}
