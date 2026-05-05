using Microsoft.AspNetCore.Mvc;

namespace FlightBookingSystem.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace FlightBookingSystem.Areas.Admin.Controllers
    {
        [Area("Admin")]
        public class PaymentController : Controller
        {
            private readonly ApplicationDbContext _context;

            public PaymentController(ApplicationDbContext context)
            {
                _context = context;
            }

            public IActionResult Index(string? search, string? status, int page = 1)
            {
                int pageSize = 10;

                var query = _context.Payments
                    .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                    .Include(p => p.Booking)
                    .ThenInclude(b => b.Flight)
                    .AsQueryable();

                // 🔍 SEARCH
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p =>
                        p.TransactionRef.Contains(search) ||
                        p.Booking.User.FName.Contains(search) ||
                        p.Booking.User.LName.Contains(search)
                    );
                }

                // 🎯 STATUS
                if (!string.IsNullOrEmpty(status) && status != "All")
                {
                    query = query.Where(p => p.PayStatus == status);
                }

                int totalItems = query.Count();

                var payments = query
                    .OrderByDescending(p => p.PayDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var vm = new PaymentListVM
                {
                    Payments = payments,
                    Search = search,
                    Status = status,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
                };

                return View(vm);
            }

            // ✅ Change Status (مثلاً من Pending لـ Completed)
            [HttpPost]
            public IActionResult UpdateStatus(int id, string status)
            {
                var payment = _context.Payments.Find(id);

                if (payment == null)
                    return NotFound();

                payment.PayStatus = status;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
        }
    }
}
