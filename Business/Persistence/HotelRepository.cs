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

        public async Task<HotelRoomDTO> CreateHotelRoom(HotelRoomRequestDTO hotelRoom)
        {
            var room = _mapper.Map<HotelRoomRequestDTO, HotelRoom>(hotelRoom);
            var addedHotelRoom = await _context.HotelRooms.AddAsync(room);
            await _context.SaveChangesAsync();

            return _mapper.Map<HotelRoom,HotelRoomDTO>(addedHotelRoom.Entity);
        }

        public async Task<HotelRoomDTO> UpdateHotelRoom(int roomId, HotelRoomRequestDTO hotelRoom)
        {
            var roomDetails = await _context.HotelRooms.FindAsync(roomId);
            var room = _mapper.Map<HotelRoomRequestDTO,HotelRoom>(hotelRoom, roomDetails);

            var updatedRoom = _context.HotelRooms.Update(room);
            await _context.SaveChangesAsync();

            return _mapper.Map<HotelRoom, HotelRoomDTO>(updatedRoom.Entity);
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

        public async Task<IEnumerable<HotelRoomDTO>> GetAllHotelRooms()
        {
            return _mapper.Map<IEnumerable<HotelRoom>, IEnumerable<HotelRoomDTO>>(await _context.HotelRooms
                .Include(x => x.HotelRoomImages)
                .Where(x => x.IsActive).ToListAsync());
        }

        public async Task<HotelRoomDTO> GetHotelRoom(int roomId)
        {
            var roomData = await _context.HotelRooms
                .Include(x => x.HotelRoomImages).FirstOrDefaultAsync(x => x.Id == roomId && x.IsActive);

            if (roomData == null)
            {
                return null;
            }
            return _mapper.Map<HotelRoom, HotelRoomDTO>(roomData);
        }

        public async Task<bool> MarkAsBooked(int roomId)
        {
            var roomData = await _context.HotelRooms
                .Include(x => x.HotelRoomImages).FirstOrDefaultAsync(x => x.Id == roomId);

            if (roomData == null)
            {
                return false;
            }

            roomData.IsBooked = true;
            var updateRoomDetails = _context.HotelRooms.Update(roomData);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
