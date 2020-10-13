using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.DataModels;

namespace HotelManagementSystem.BlazorWasm.Core
{
    public interface IAuthenticationService
    {
        Task<UserDTO> SignUp();
    }
}
