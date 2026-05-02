namespace FlightBookingSystem.Models;
public class Admin
{
    public int AdminId { get; set; }
    public int UserId { get; set; }
    public string AdminLevel { get; set; } = null!;
    public string Permissions { get; set; } = null!;

    // Navigation
    public User User { get; set; } = null!;
}