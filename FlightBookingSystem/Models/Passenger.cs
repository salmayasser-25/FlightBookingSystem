namespace FlightBookingSystem.Models;
using System.ComponentModel.DataAnnotations;

public class Passenger
{
    [Key]
    public int PassengerId { get; set; }
    public string FullName { get; set; } = null!;
    public string PassportNumber { get; set; } = null!;
    public int Age { get; set; }
    public int BookingId { get; set; }
    public int FlightId { get; set; }
    public int ClassId { get; set; }

    // Navigation
    public Booking Booking { get; set; } = null!;
    public FlightSeatClass FlightSeatClass { get; set; } = null!;
    public Ticket? Ticket { get; set; }
}