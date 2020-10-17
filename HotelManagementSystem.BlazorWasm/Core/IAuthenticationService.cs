using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.DataModels;

namespace HotelManagementSystem.BlazorWasm.Core
{
    public interface IAuthenticationService
    {
        Task<SuccessModel> SignUp(UserRequestDTO model);
        Task<UserDTO> SignIn(AuthenticationDTO model);
        Task<SuccessModel> ForgotPassword(ForgotPasswordDTO model);
        Task<SuccessModel> ResetPassword(PasswordResetDTO model);
        Task<SuccessModel> ConfirmEmail(ConfirmEmailDTO model);
        Task Logout();
        UserDTO User { get; }
        bool IsLoggedIn { get; }
        Task Initialize();
    }
}
