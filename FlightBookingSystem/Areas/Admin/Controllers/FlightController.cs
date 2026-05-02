using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingSystem.Areas.Admin.Controllers
{
    [Area(SD.Admin_Area)]
    public class FlightController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlightController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search, string? status, int page = 1)
        {
            int pageSize = 10;

            var query = _context.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .AsQueryable();

            // 🔍 Search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(f =>
                    f.FlightNum.Contains(search) ||
                    f.DepartureAirport.Name.Contains(search) ||
                    f.ArrivalAirport.Name.Contains(search)
                );
            }

            // 🎯 Status filter (لو عندك Status column)
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(f => f.Status == status);
            }

            int totalItems = query.Count();

            var flights = query
                .OrderByDescending(f => f.DepartureTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new FlightListVM
            {
                Flights = flights,
                Search = search,
                Status = status,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Airports = _context.Airports.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Flight flight)
        {
            if (ModelState.IsValid)
            {
                _context.Flights.Add(flight);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Airports = _context.Airports.ToList();
            return View(flight);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var flight = _context.Flights.Find(id);
            if (flight == null) return NotFound();

            ViewBag.Airports = _context.Airports.ToList();
            return View(flight);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Flight flight)
        {
            if (ModelState.IsValid)
            {
                _context.Flights.Update(flight);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Airports = _context.Airports.ToList();
            return View(flight);
        }
    }

}