using System.Threading.Tasks;
using Business.DataModels;

namespace HotelManagementSystem.BlazorServer.Services
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
        Task<bool> IsUserAuthorized();
    }
}
