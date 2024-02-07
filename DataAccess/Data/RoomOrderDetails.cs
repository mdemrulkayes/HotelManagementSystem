using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Data
{
    public class RoomOrderDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string StripeSessionId { get; set; }
        [Required]
        public DateTime CheckInDate { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime CheckOutDate { get; set; } = DateTime.UtcNow.AddDays(1);
        [Required]
        public long TotalCost { get; set; }
        [Required]
        public int RoomId { get; set; }

        public bool IsPaymentSuccessful { get; set; } = false;
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }

        [ForeignKey("RoomId")]
        public HotelRoom HotelRoom { get; set; }
    }
}
