namespace FlightBookingSystem.ViewModels
{
    public class FlightListVM
    {
      
            public List<Flight> Flights { get; set; } = new();

            public string? Search { get; set; }
            public string? Status { get; set; }

            public int PageNumber { get; set; }
            public int PageSize { get; set; }

            public int TotalPages { get; set; }
            public int TotalItems { get; set; }
        
    }
}
