﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagementSystem.BlazorServer.Models.ViewModels
{
    public class ResetPasswordVm
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required(ErrorMessage = "New password is required")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm new Password is required.")]
        [CompareProperty("NewPassword", ErrorMessage = "New Password & Confirm Password must be same.")]
        public string ConfirmNewPassword { get; set; }
    }
}
