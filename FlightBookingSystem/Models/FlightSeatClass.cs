namespace FlightBookingSystem.Models;
public class FlightSeatClass
{
    public int FlightId { get; set; }
    public int ClassId { get; set; }
    public int AvailableSeats { get; set; }
    public decimal FinalPrice { get; set; }

    // Navigation
    public Flight Flight { get; set; } = null!;
    public SeatClass SeatClass { get; set; } = null!;
    public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
}