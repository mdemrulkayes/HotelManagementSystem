using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Business.Core;
using DataAccess.Data;

namespace Business.Persistence
{
    public class RoomOrderDetailsRepository: IRoomOrderDetailsRepository
    {
        private readonly CoreDbContext _context;

        public RoomOrderDetailsRepository(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<RoomOrderDetails> Create(RoomOrderDetails details)
        {
            var result = await _context.RoomOrderDetails.AddAsync(details);
            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<RoomOrderDetails> MarkAsPaymentSuccessful(int id)
        {
            var data = await _context.RoomOrderDetails.FindAsync(id);
            if (data ==null)
            {
                return null;
            }

            data.IsPaymentSuccessful = true;
            var markAsPaymentSuccessful = _context.RoomOrderDetails.Update(data);
            await _context.SaveChangesAsync();

            return markAsPaymentSuccessful.Entity;
        }
    }
}
