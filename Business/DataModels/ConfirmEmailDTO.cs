using System.ComponentModel.DataAnnotations;

namespace Business.DataModels
{
    public class ConfirmEmailDTO
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
