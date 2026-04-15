using System.ComponentModel.DataAnnotations;

public class Ticket
{
    [Key]
    public int TicketId { get; set; }
    public int BookingId { get; set; }
    public int PassengerId { get; set; }
    public DateTime IssueDate { get; set; }
    public string SeatNum { get; set; } = null!;
    public string QrCode { get; set; } = null!;

    // Navigation
    public Booking Booking { get; set; } = null!;
    public Passenger Passenger { get; set; } = null!;
}