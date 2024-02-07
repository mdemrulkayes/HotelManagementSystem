using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Data
{
    public class HotelRoom
    {
        public HotelRoom()
        {
            this.HotelRoomImages = new HashSet<HotelRoomImage>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public bool IsDeleted { get; set; } = false;
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string DeletedBy { get; set; }
        public DateTime DeletedDate { get; set; }

        
        public virtual ICollection<HotelRoomImage> HotelRoomImages { get; set; }
    }
}
