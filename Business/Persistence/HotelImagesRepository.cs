using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Core;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace Business.Persistence
{
    public class HotelImagesRepository: IHotelImagesRepository
    {
        private readonly CoreDbContext _context;

        public HotelImagesRepository(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateHotelRoomImage(HotelRoomImage image)
        {
            await _context.HotelRoomImages.AddAsync(image);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteHotelImageByHotelRoomId(int roomId)
        {
            var allImages = await _context.HotelRoomImages.Where(x => x.RoomId == roomId).ToListAsync();
            _context.HotelRoomImages.RemoveRange(allImages);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteHotelImageByImageId(int id)
        {
            var image = await _context.HotelRoomImages.FindAsync(id);
            _context.HotelRoomImages.Remove(image);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<HotelRoomImage>> GetHotelRoomImages(int roomId)
        {
            return await _context.HotelRoomImages
                .Include(x => x.HotelRoom)
                .Where(x => x.RoomId == roomId).ToListAsync();
        }
    }
}
