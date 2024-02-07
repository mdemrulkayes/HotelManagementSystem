using System;

namespace HotelManagementSystem.BlazorWasm.Models.ViewModels
{
    public class HomeModelVm
    {
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(1);
        public double TotalDay { get; set; }
    }
}
