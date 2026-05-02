namespace FlightBookingSystem.Models;
using System.ComponentModel.DataAnnotations;

public class SeatClass
{
    [Key]
    public int ClassId { get; set; }
    public string ClassName { get; set; } = null!;
    public decimal ClassMultiplier { get; set; }

    // Navigation
    public ICollection<FlightSeatClass> FlightSeatClasses { get; set; } = new List<FlightSeatClass>();
}