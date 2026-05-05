namespace FlightBookingSystem.ViewModels
{
    public class PaymentListVM
    {
        public List<Payment> Payments { get; set; } = new();

        public string? Search { get; set; }
        public string? Status { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
