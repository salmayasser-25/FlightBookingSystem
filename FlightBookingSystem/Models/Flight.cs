namespace FlightBookingSystem.Models;
using System.ComponentModel.DataAnnotations;

public class Flight
{
    [Key]
    public int FlightId { get; set; }

    public string FlightNum { get; set; } = null!;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal BasePrice { get; set; }
    public string Status { get; set; } = null!;

    public int DepartureAirportId { get; set; }
    public int ArrivalAirportId { get; set; }

    
    public int AvailableSeats { get; set; }

    // Navigation
    public Airport DepartureAirport { get; set; } = null!;
    public Airport ArrivalAirport { get; set; } = null!;

    public ICollection<FlightSeatClass> FlightSeatClasses { get; set; } = new List<FlightSeatClass>();
    public ICollection<FlightRule> FlightRules { get; set; } = new List<FlightRule>();
}