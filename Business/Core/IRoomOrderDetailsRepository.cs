using System.Threading.Tasks;
using DataAccess.Data;

namespace Business.Core
{
    public interface IRoomOrderDetailsRepository
    {
        public Task<RoomOrderDetails> Create(RoomOrderDetails details);
        public Task<RoomOrderDetails> MarkAsPaymentSuccessful(int id);
    }
}
