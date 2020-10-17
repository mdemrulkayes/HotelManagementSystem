using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using DataAccess.Data;

namespace Business.DataModels
{
    public class HotelRoomDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter room name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter Occupancy")]
        public int Occupancy { get; set; }
        [Required(ErrorMessage = "Please enter regular rate")]
        public decimal RegularRate { get; set; }
        [Required(ErrorMessage = "Please enter room details")]
        public string Details { get; set; }
        [Required(ErrorMessage = "Please enter room size")]
        public decimal SqrFt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsBooked { get; set; } = false;

        public virtual ICollection<HotelRoomImageDTO> HotelRoomImages { get; set; }
    }
}
