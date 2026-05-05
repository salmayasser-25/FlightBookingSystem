using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index(string? search, string? status, int page = 1)
        {
            int pageSize = 10;

            var query = _context.Bookings
                .Include(b => b.Flight).ThenInclude(f => f.DepartureAirport)
                .Include(b => b.Flight).ThenInclude(f => f.ArrivalAirport)
                .Include(b => b.User)
                .Include(b => b.Passengers)
                .AsQueryable();

            // 🔍 SEARCH
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b =>
                    b.User.FName.Contains(search) ||
                    b.User.LName.Contains(search) ||
                    b.Flight.FlightNum.Contains(search)
                );
            }

            // 🎯 STATUS
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(b => b.Status == status);
            }

            int totalItems = query.Count();

            var bookings = query
                .OrderByDescending(b => b.BookingDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new BookingListVM
            {
                Bookings = bookings,
                Search = search,
                Status = status,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };

            return View(vm);
        }

        // ================= DETAILS =================
        public IActionResult Details(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Flight)
                .ThenInclude(f => f.DepartureAirport)
                .Include(b => b.Flight)
                .ThenInclude(f => f.ArrivalAirport)
                .Include(b => b.User)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // ================= CANCEL BOOKING =================
        [HttpPost]
        public IActionResult Cancel(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Flight)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            booking.Status = "Cancelled";

            // 🔁 يرجع المقاعد للفلايت
            booking.Flight.AvailableSeats += booking.Passengers.Count;

            _context.SaveChanges();

            TempData["Success"] = "Booking cancelled successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}