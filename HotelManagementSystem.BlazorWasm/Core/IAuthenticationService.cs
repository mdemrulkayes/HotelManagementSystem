using System.Threading.Tasks;
using Business.DataModels;
using HotelManagementSystem.BlazorWasm.Models.ViewModels;

namespace HotelManagementSystem.BlazorWasm.Core
{
    public interface IAuthenticationService
    {
        Task<SuccessModel> SignUp(UserRequestDTO model);
        Task<UserDTO> SignIn(AuthenticationDTO model);
        Task<UserDTO> SigninInWithFacebook(FbResponseVm model);
        Task<SuccessModel> ForgotPassword(ForgotPasswordDTO model);
        Task<SuccessModel> ResetPassword(PasswordResetDTO model);
        Task<SuccessModel> ConfirmEmail(ConfirmEmailDTO model);
        Task Logout();
        UserDTO User { get; }
        bool IsLoggedIn { get; }
        Task Initialize();
    }
}
