using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FlightBookingSystem.Areas.Admin.Controllers
{
    [Area(SD.Admin_Area)]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // 📊 Basic Stats
            var totalBookings = _context.Bookings.Count();

            var totalRevenue = _context.Payments
                .Where(p => p.PayStatus == "Completed")
                .Sum(p => (decimal?)p.Amount) ?? 0;

            var totalUsers = _context.Users.Count();

            var totalFlights = _context.Flights.Count();

            // 🆕 Latest Bookings (Simple & Clean)
            var latestBookings = _context.Bookings
                .Include(b => b.User)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToList();

            // ✈️ Top Flights (clean logic)
            var topFlightIds = _context.Passengers
                .GroupBy(p => p.FlightId)
                .Select(g => new
                {
                    FlightId = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .Select(x => x.FlightId)
                .ToList();

            var topFlights = _context.Flights
                .Where(f => topFlightIds.Contains(f.FlightId))
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .ToList();

            // 📦 ViewModel
            var vm = new DashboardVM
            {
                TotalBookings = totalBookings,
                TotalRevenue = totalRevenue,
                TotalUsers = totalUsers,
                TotalFlights = totalFlights,
                LatestBookings = latestBookings,
                TopFlights = topFlights
            };

            return View(vm);
        }
    }
}
