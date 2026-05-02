namespace FlightBookingSystem.ViewModels
{
 
        public class DashboardVM
        {
            public int TotalBookings { get; set; }
            public decimal TotalRevenue { get; set; }
            public int TotalUsers { get; set; }
            public int TotalFlights { get; set; }

            public List<Booking> LatestBookings { get; set; }
            public List<Flight> TopFlights { get; set; }
        }
    
}
