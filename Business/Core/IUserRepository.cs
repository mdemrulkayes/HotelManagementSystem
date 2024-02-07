using System.Collections.Generic;
using System.Threading.Tasks;
using Business.DataModels;
using Microsoft.AspNetCore.Identity;

namespace Business.Core
{
    public interface IUserRepository
    {
        Task<UserDTO> GetUserAsync(string userId);
        Task<UserDTO> GetUserByNameAsync(string userName);
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<IdentityResult> CreateUser(UserRequestDTO user);
        Task<IdentityResult> UpdateUser(string id, UserDTO user);
        Task<IdentityResult> DeleteUser(string id);
        Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordModel model);
    }
}
