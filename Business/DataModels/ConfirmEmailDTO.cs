using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
