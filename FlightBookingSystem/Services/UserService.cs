using System.Security.Cryptography;
using System.Text;
using FlightBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingSystem.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext db, ILogger<UserService> logger)
        {
            _db = db;
            _logger = logger;
        }

        // SECURITY: hashing scheme inherited from AccountController; migration tracked separately
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public async Task<ProfileViewModel?> GetProfileAsync(int userId)
        {
            var user = await _db.Users
                .Include(u => u.RewardAccount)
                .Include(u => u.Admin)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return null;

            return new ProfileViewModel
            {
                UserId = user.UserId,
                FName = user.FName,
                LName = user.LName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                CreatedDay = user.CreatedDay,
                CreatedMonth = user.CreatedMonth,
                CreatedYear = user.CreatedYear,
                RewardPoints = user.RewardAccount?.PointsBalance,
                IsAdmin = user.Admin != null
            };
        }

        public async Task<EditProfileViewModel?> GetEditModelAsync(int userId)
        {
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return null;

            return new EditProfileViewModel
            {
                FName = user.FName,
                LName = user.LName,
                Email = user.Email
            };
        }

        public async Task<bool> UpdateProfileAsync(int userId, EditProfileViewModel model)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return false;

            user.FName = model.FName.Trim();
            user.LName = model.LName.Trim();

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string? Error)> ChangePasswordAsync(int userId, ChangePasswordViewModel model)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return (false, "Account not found.");

            if (user.Password != HashPassword(model.CurrentPassword))
                return (false, "Current password is incorrect.");

            if (model.CurrentPassword == model.NewPassword)
                return (false, "New password must be different from your current password.");

            user.Password = HashPassword(model.NewPassword);
            await _db.SaveChangesAsync();
            return (true, null);
        }

        public Task<int> CountActiveBookingsAsync(int userId)
        {
            return _db.Bookings
                .Where(b => b.UserId == userId
                    && b.Status.ToLower() != "cancelled"
                    && b.Status.ToLower() != "completed")
                .CountAsync();
        }

        public async Task<(bool Success, string? Error, int ActiveBookingCount)> DeleteAccountAsync(int userId, string passwordPlain)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return (false, "Account not found.", 0);

            if (user.Password != HashPassword(passwordPlain))
                return (false, "Password is incorrect.", 0);

            var hasAdmin = await _db.Admins.AnyAsync(a => a.UserId == userId);
            if (hasAdmin)
                return (false, "Admin accounts cannot be deleted from this page. Please contact support.", 0);

            var activeCount = await CountActiveBookingsAsync(userId);
            if (activeCount > 0)
                return (false, $"You have {activeCount} active booking(s). Please cancel them before deleting your account.", activeCount);

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var bookingIds = await _db.Bookings
                    .Where(b => b.UserId == userId)
                    .Select(b => b.BookingId)
                    .ToListAsync();

                if (bookingIds.Count > 0)
                {
                    var tickets = await _db.Tickets
                        .Where(t => bookingIds.Contains(t.BookingId))
                        .ToListAsync();
                    if (tickets.Count > 0) _db.Tickets.RemoveRange(tickets);

                    var passengers = await _db.Passengers
                        .Where(p => bookingIds.Contains(p.BookingId))
                        .ToListAsync();
                    if (passengers.Count > 0) _db.Passengers.RemoveRange(passengers);

                    var payments = await _db.Payments
                        .Where(p => bookingIds.Contains(p.BookingId))
                        .ToListAsync();
                    if (payments.Count > 0) _db.Payments.RemoveRange(payments);

                    var bookings = await _db.Bookings
                        .Where(b => b.UserId == userId)
                        .ToListAsync();
                    _db.Bookings.RemoveRange(bookings);
                }

                var reward = await _db.RewardAccounts
                    .Include(r => r.Transactions)
                    .FirstOrDefaultAsync(r => r.UserId == userId);
                if (reward != null)
                {
                    if (reward.Transactions.Count > 0)
                        _db.PointsTransactions.RemoveRange(reward.Transactions);
                    _db.RewardAccounts.Remove(reward);
                }

                _db.Users.Remove(user);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                _logger.LogWarning("Account deleted for UserId={UserId} ({Email})", userId, user.Email);
                return (true, null, 0);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to delete account for UserId={UserId}", userId);
                return (false, "Could not delete the account due to a server error. Please try again.", 0);
            }
        }

        public async Task<MyBookingsViewModel> GetBookingsAsync(int userId)
        {
            var bookings = await _db.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .Select(b => new
                {
                    b.BookingId,
                    b.BookingDate,
                    b.Status,
                    b.TotalPrice,
                    PassengerCount = b.Passengers.Count,
                    TicketCount = b.Tickets.Count,
                    FirstFlightId = b.Passengers
                        .Select(p => (int?)p.FlightId)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var flightIds = bookings
                .Where(b => b.FirstFlightId.HasValue)
                .Select(b => b.FirstFlightId!.Value)
                .Distinct()
                .ToList();

            var routes = await _db.Flights
                .AsNoTracking()
                .Where(f => flightIds.Contains(f.FlightId))
                .Select(f => new
                {
                    f.FlightId,
                    Departure = f.DepartureAirport.IataCode,
                    Arrival = f.ArrivalAirport.IataCode
                })
                .ToDictionaryAsync(x => x.FlightId, x => $"{x.Departure} → {x.Arrival}");

            var vm = new MyBookingsViewModel
            {
                Bookings = bookings.Select(b => new BookingSummaryViewModel
                {
                    BookingId = b.BookingId,
                    BookingDate = b.BookingDate,
                    Status = b.Status,
                    TotalPrice = b.TotalPrice,
                    PassengerCount = b.PassengerCount,
                    TicketCount = b.TicketCount,
                    RouteSummary = b.FirstFlightId.HasValue && routes.TryGetValue(b.FirstFlightId.Value, out var r) ? r : null
                }).ToList()
            };

            return vm;
        }

        public async Task<UserDashboardViewModel?> GetDashboardAsync(int userId)
        {
            var user = await _db.Users
                .Include(u => u.RewardAccount)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return null;

            var rawBookings = await _db.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .Select(b => new
                {
                    b.BookingId,
                    b.BookingDate,
                    b.Status,
                    FirstFlightId = b.Passengers
                        .Select(p => (int?)p.FlightId)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var flightIds = rawBookings
                .Where(b => b.FirstFlightId.HasValue)
                .Select(b => b.FirstFlightId!.Value)
                .Distinct()
                .ToList();

            var flightInfo = await _db.Flights
                .AsNoTracking()
                .Where(f => flightIds.Contains(f.FlightId))
                .Select(f => new
                {
                    f.FlightId,
                    f.FlightNum,
                    Departure = f.DepartureAirport.IataCode,
                    ArrivalAirportId = f.ArrivalAirportId,
                    Arrival = f.ArrivalAirport.IataCode
                })
                .ToDictionaryAsync(x => x.FlightId);

            var recent = rawBookings.Take(5).Select(b =>
            {
                var info = b.FirstFlightId.HasValue && flightInfo.TryGetValue(b.FirstFlightId.Value, out var f) ? f : null;
                return new RecentFlightSummary
                {
                    BookingId = b.BookingId,
                    BookingDate = b.BookingDate,
                    Status = b.Status,
                    DepartureCode = info?.Departure ?? "—",
                    ArrivalCode = info?.Arrival ?? "—",
                    FlightNumber = info?.FlightNum ?? "",
                    Pnr = $"AF{b.BookingId:D6}"
                };
            }).ToList();

            var completedFlights = rawBookings.Count(b =>
                string.Equals(b.Status, "completed", StringComparison.OrdinalIgnoreCase));

            var distinctArrivals = rawBookings
                .Where(b => b.FirstFlightId.HasValue && flightInfo.ContainsKey(b.FirstFlightId.Value))
                .Select(b => flightInfo[b.FirstFlightId!.Value].ArrivalAirportId)
                .Distinct()
                .Count();

            return new UserDashboardViewModel
            {
                FName = user.FName,
                LName = user.LName,
                Email = user.Email,
                RewardPoints = user.RewardAccount?.PointsBalance ?? 0,
                FlightsTaken = completedFlights,
                CitiesVisited = distinctArrivals,
                RecentFlights = recent
            };
        }
    }
}
