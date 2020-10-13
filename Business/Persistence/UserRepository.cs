using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business.Core;
using Business.DataModels;
using DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Persistence
{
    public class UserRepository: IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserRepository(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserDTO> GetUserAsync(string userId)
        {
            var result = await _userManager.Users.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == userId);
            return _mapper.Map<ApplicationUser, UserDTO>(result);
        }

        public async Task<UserDTO> GetUserByNameAsync(string userName)
        {
            var result = await _userManager.Users.FirstOrDefaultAsync(x => !x.IsDeleted && x.UserName.ToUpper() == userName.ToUpper());
            return _mapper.Map<ApplicationUser, UserDTO>(result);
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            var allUsers = await _userManager.Users.Where(x => !x.IsDeleted)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserDTO>>(allUsers);
        }

        public async Task<IdentityResult> CreateUser(UserRequestDTO user)
        {
            var mappedUser = _mapper.Map<UserRequestDTO, ApplicationUser>(user);
            mappedUser.EmailConfirmed = true;
            mappedUser.PhoneNumberConfirmed = true;
            mappedUser.IsDeleted = false;

            var result = await _userManager.CreateAsync(mappedUser, user.Password);
            
            return result;
        }

        public async Task<IdentityResult> UpdateUser(string id, UserDTO user)
        {
            var userDetails = await _userManager.FindByIdAsync(id);
            var result = new IdentityResult();
            if (userDetails != null)
            {
                var updatedUser = _mapper.Map<UserDTO, ApplicationUser>(user, userDetails);

                result = await _userManager.UpdateAsync(updatedUser);
            }
            return result;

        }

        public async Task<IdentityResult> DeleteUser(string id)
        {
            var userDetails = await _userManager.FindByIdAsync(id);
            var result = new IdentityResult();
            if (userDetails != null)
            {
                userDetails.IsDeleted = true;
                result = await _userManager.UpdateAsync(userDetails);
            }

            return result;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordModel model)
        {
            var userDetails = await _userManager.FindByIdAsync(userId);
            var result = new IdentityResult();
            if (userDetails != null)
            {
                result = await _userManager.ChangePasswordAsync(userDetails, model.OldPassword, model.NewPassword);
            }

            return result;
        }
    }
}
