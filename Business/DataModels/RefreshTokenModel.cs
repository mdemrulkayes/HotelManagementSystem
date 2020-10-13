﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.DataModels
{
    public class RefreshTokenModel
    {
        [Required(ErrorMessage = "Expired token is required")]
        public string ExpiredToken { get; set; }
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; }
    }
}
