namespace FlightBookingSystem.Models;
using System.ComponentModel.DataAnnotations;

public class Payment
{
    [Key]
    public int PayId { get; set; }
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public string PayMethod { get; set; } = null!;
    public string PayStatus { get; set; } = null!;
    public DateTime PayDate { get; set; }
    public string TransactionRef { get; set; } = null!;

    // Navigation
    public Booking Booking { get; set; } = null!;
}