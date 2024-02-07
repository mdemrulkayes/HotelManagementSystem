using System.Collections.Generic;
using System.Threading.Tasks;
using Business.DataModels;

namespace Business.Core
{
    public interface IHotelRepository
    {
        public Task<HotelRoomDTO> CreateHotelRoom(HotelRoomRequestDTO hotelRoom);
        public Task<HotelRoomDTO> UpdateHotelRoom(int roomId, HotelRoomRequestDTO hotelRoom);
        public Task<int> DeleteHotelRoom(int roomId,string userId);
        public Task<IEnumerable<HotelRoomDTO>> GetAllHotelRooms();
        public Task<HotelRoomDTO> GetHotelRoom(int roomId);
        public Task<bool> MarkAsBooked(int roomId);
        public Task<HotelRoomDTO> IsSameNameRoomAlreadyExists(string name);
    }
}
