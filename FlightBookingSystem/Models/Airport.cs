namespace FlightBookingSystem.Models;
using System.ComponentModel.DataAnnotations;

public class Airport
{
    [Key]
    public int AirportId { get; set; }
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string IataCode { get; set; } = null!;

    // Navigation
    public ICollection<Flight> DepartureFlights { get; set; } = new List<Flight>();
    public ICollection<Flight> ArrivalFlights { get; set; } = new List<Flight>();
}