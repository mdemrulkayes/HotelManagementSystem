using System.ComponentModel.DataAnnotations;

namespace Business.DataModels
{
    public class PasswordResetDTO
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required(ErrorMessage = "New password is required")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm new Password is required.")]
        [Compare("NewPassword",ErrorMessage = "New Password & Confirm Password must be same.")]
        public string ConfirmNewPassword { get; set; }
    }
}
