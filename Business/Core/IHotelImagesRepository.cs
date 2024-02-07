using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Data;

namespace Business.Core
{
    public interface IHotelImagesRepository
    {
        public Task<int> CreateHotelRoomImage(HotelRoomImage image);
        public Task<int> DeleteHotelImageByHotelRoomId(int roomId);
        public Task<int> DeleteHotelImageByImageId(int id);
        public Task<IEnumerable<HotelRoomImage>> GetHotelRoomImages(int roomId);
    }
}
