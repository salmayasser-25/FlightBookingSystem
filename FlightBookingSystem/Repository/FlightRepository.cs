using Microsoft.EntityFrameworkCore;

namespace FlightBookingSystem.Repository.IRepository
{
    public class FlightRepository : Repository<Flight>, IFlightRepository
    {
        private readonly ApplicationDbContext _context;

        public FlightRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Flight>> SearchFlights(string from, string to, DateTime date, int classId)
        {
            return await _context.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.FlightSeatClasses)
                .Where(f =>
                    f.DepartureAirport.City == from &&
                    f.ArrivalAirport.City == to &&
                    f.DepartureTime.Date == date.Date &&
                    f.FlightSeatClasses.Any(c => c.ClassId == classId && c.AvailableSeats > 0))
                .ToListAsync();
        }
        public async Task<Flight?> GetFlightDetails(int id)
        {
            return await _context.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.FlightSeatClasses)
                .ThenInclude(fsc => fsc.SeatClass)
                .FirstOrDefaultAsync(f => f.FlightId == id);
        }

        public async Task<int> GetAvailableSeats(int flightId, int classId)
        {
            return await _context.FlightSeatClasses
                .Where(f => f.FlightId == flightId && f.ClassId == classId)
                .Select(f => f.AvailableSeats)
                .FirstOrDefaultAsync();
        }
         
    }
}
