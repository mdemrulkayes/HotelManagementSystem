namespace Business.DataModels
{
    public class StripePaymentDTO
    {
        public string ProductName { get; set; }
        public long Amount { get; set; }
        //public string UserId { get; set; }
        public string ImageUrl { get; set; }
    }
}
