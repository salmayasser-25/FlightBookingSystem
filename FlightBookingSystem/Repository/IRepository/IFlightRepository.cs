namespace FlightBookingSystem.Repository.IRepository
{
    public interface IFlightRepository : IRepository<Flight>
    {
        Task<List<Flight>> SearchFlights(string from, string to, DateTime date, int classId);
        Task<Flight?> GetFlightDetails(int id);
        Task<int> GetAvailableSeats(int flightId, int classId);
    }
}
