using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Business.DataModels;
using DataAccess.Data;

namespace Business.Core
{
    public interface IHotelRepository
    {
        public Task<HotelRoom> CreateHotelRoom(HotelRoomRequestDTO hotelRoom);
        public Task<HotelRoom> UpdateHotelRoom(int roomId, HotelRoomRequestDTO hotelRoom);
        public Task<int> DeleteHotelRoom(int roomId);
        public Task<IEnumerable<HotelRoom>> GetAllHotelRooms();
        public Task<HotelRoom> GetHotelRoom(int roomId);
    }
}
