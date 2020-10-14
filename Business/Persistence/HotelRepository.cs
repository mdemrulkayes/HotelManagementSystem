using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business.Core;
using Business.DataModels;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace Business.Persistence
{
    public class HotelRepository: IHotelRepository
    {
        private readonly CoreDbContext _context;
        private readonly IMapper _mapper;

        public HotelRepository(CoreDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<HotelRoom> CreateHotelRoom(HotelRoomRequestDTO hotelRoom)
        {
            var room = _mapper.Map<HotelRoomRequestDTO, HotelRoom>(hotelRoom);
            var addedHotelRoom = await _context.HotelRooms.AddAsync(room);
            await _context.SaveChangesAsync();

            return addedHotelRoom.Entity;
        }

        public async Task<HotelRoom> UpdateHotelRoom(int roomId, HotelRoomRequestDTO hotelRoom)
        {
            var roomDetails = await _context.HotelRooms.FindAsync(roomId);
            var room = _mapper.Map<HotelRoomRequestDTO,HotelRoom>(hotelRoom, roomDetails);

            var updatedRoom = _context.HotelRooms.Update(room);
            await _context.SaveChangesAsync();

            return updatedRoom.Entity;
        }

        public async Task<int> DeleteHotelRoom(int roomId)
        {
            var roomDetails = await _context.HotelRooms.FindAsync(roomId);
            if (roomDetails != null)
            {
                _context.HotelRooms.Remove(roomDetails);
                return await _context.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<IEnumerable<HotelRoom>> GetAllHotelRooms()
        {
            return await _context.HotelRooms
                .Include(x => x.HotelRoomImages)
                .Where(x => x.IsActive).ToListAsync();
        }

        public async Task<HotelRoom> GetHotelRoom(int roomId)
        {
            return await _context.HotelRooms.FindAsync(roomId);
        }
    }
}
