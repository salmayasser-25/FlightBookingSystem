namespace FlightBookingSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

public class Booking
{
    [Key]
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalPrice { get; set; }
    public bool DiscountApplied { get; set; }
    public int PointsUsed { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Payment? Payment { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
}